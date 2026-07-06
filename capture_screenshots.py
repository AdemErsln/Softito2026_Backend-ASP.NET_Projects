import os
import subprocess
import time
import json
import socket
import sys
import re

# Configuration
CHROME_PATH = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
OUTPUT_DIR = "screenshots"
PROJECTS_DIR = "."
WAIT_TIMEOUT = 60  # Increased to 60 seconds for slow cold builds

# Ensure output directory exists
os.makedirs(OUTPUT_DIR, exist_ok=True)

def kill_existing_dotnet():
    print("Killing any existing dotnet processes to free up ports...")
    if sys.platform == "win32":
        subprocess.run(["taskkill", "/F", "/IM", "dotnet.exe"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
        subprocess.run(["taskkill", "/F", "/IM", "iisexpress.exe"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    else:
        subprocess.run(["killall", "-9", "dotnet"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
    time.sleep(2.0)

def find_web_projects():
    web_projects = []
    # Scan all directories
    for root, dirs, files in os.walk(PROJECTS_DIR):
        # Ignore build artifact folders
        if any(ignored in root for ignored in ["bin", "obj", ".vs", "node_modules"]):
            continue
        for file in files:
            if file.endswith(".csproj"):
                csproj_path = os.path.join(root, file)
                try:
                    with open(csproj_path, "r", encoding="utf-8-sig") as f:
                        content = f.read()
                        if 'Sdk="Microsoft.NET.Sdk.Web"' in content:
                            web_projects.append(csproj_path)
                except Exception as e:
                    print(f"Error reading {csproj_path}: {e}")
    return web_projects

def parse_url_from_launch_settings(project_dir):
    settings_path = os.path.join(project_dir, "Properties", "launchSettings.json")
    if not os.path.exists(settings_path):
        return None
    
    try:
        with open(settings_path, "r", encoding="utf-8-sig") as f:
            data = json.load(f)
            profiles = data.get("profiles", {})
            for profile_name, profile_data in profiles.items():
                url_str = profile_data.get("applicationUrl")
                if url_str:
                    urls = [u.strip() for u in re.split(r'[;,]', url_str) if u.strip()]
                    # Prefer http over https to avoid SSL warnings in headless chrome
                    http_urls = [u for u in urls if u.startswith("http://")]
                    if http_urls:
                        return http_urls[0]
                    elif urls:
                        return urls[0]
    except Exception as e:
        print(f"Error parsing launchSettings.json in {project_dir}: {e}")
    return None

def find_mvc_routes(project_dir):
    routes = ["/"]  # Base URL
    views_dir = os.path.join(project_dir, "Views")
    areas_dir = os.path.join(project_dir, "Areas")
    pages_dir = os.path.join(project_dir, "Pages")
    
    # Razor Pages
    if os.path.exists(pages_dir):
        for root, dirs, files in os.walk(pages_dir):
            if "Shared" in root:
                continue
            for file in files:
                if file.endswith(".cshtml") and not file.startswith("_"):
                    rel_path = os.path.relpath(os.path.join(root, file), pages_dir)
                    rel_route = os.path.splitext(rel_path)[0].replace(os.sep, "/")
                    
                    if rel_route.lower() == "index":
                        continue  # already covered by "/"
                    
                    if rel_route.lower().endswith("/index"):
                        route = "/" + rel_route[:-6]
                    else:
                        route = "/" + rel_route
                        
                    base_name = os.path.splitext(file)[0].lower()
                    if base_name in ["duzenle", "sil", "edit", "delete", "details", "update"]:
                        route += "/1"
                        
                    routes.append(route)
                    
    # Standard Views
    if os.path.exists(views_dir):
        for controller in os.listdir(views_dir):
            controller_path = os.path.join(views_dir, controller)
            if os.path.isdir(controller_path) and controller.lower() != "shared":
                for view_file in os.listdir(controller_path):
                    if view_file.endswith(".cshtml") and not view_file.startswith("_"):
                        view_name = os.path.splitext(view_file)[0]
                        if controller.lower() == "home" and view_name.lower() == "index":
                            continue
                        
                        if view_name.lower() == "index":
                            routes.append(f"/{controller}")
                        else:
                            if view_name.lower() in ["edit", "delete", "details", "update"]:
                                routes.append(f"/{controller}/{view_name}/1")
                            else:
                                routes.append(f"/{controller}/{view_name}")
                            
    # Areas (e.g. Admin)
    if os.path.exists(areas_dir):
        for area in os.listdir(areas_dir):
            area_path = os.path.join(areas_dir, area)
            if os.path.isdir(area_path):
                area_views_dir = os.path.join(area_path, "Views")
                if os.path.exists(area_views_dir):
                    for controller in os.listdir(area_views_dir):
                        controller_path = os.path.join(area_views_dir, controller)
                        if os.path.isdir(controller_path) and controller.lower() != "shared":
                            for view_file in os.listdir(controller_path):
                                if view_file.endswith(".cshtml") and not view_file.startswith("_"):
                                    view_name = os.path.splitext(view_file)[0]
                                    if controller.lower() == "home" and view_name.lower() == "index":
                                        routes.append(f"/{area}")
                                    elif view_name.lower() == "index":
                                        routes.append(f"/{area}/{controller}")
                                    else:
                                        if view_name.lower() in ["edit", "delete", "details", "update"]:
                                            routes.append(f"/{area}/{controller}/{view_name}/1")
                                        else:
                                            routes.append(f"/{area}/{controller}/{view_name}")
                                        
    # Remove duplicates
    unique_routes = sorted(list(set(routes)))
    return unique_routes

def is_port_open(host, port):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.settimeout(1.0)
        try:
            s.connect((host, port))
            return True
        except Exception:
            return False

def wait_for_server(url, timeout=WAIT_TIMEOUT):
    match = re.match(r"https?://([^:/]+)(?::(\d+))?", url)
    if not match:
        return False
    host = match.group(1)
    port_str = match.group(2)
    port = int(port_str) if port_str else (443 if url.startswith("https") else 80)
    
    print(f"Waiting for server on {host}:{port} (timeout: {timeout}s)...")
    start_time = time.time()
    while time.time() - start_time < timeout:
        if is_port_open(host, port):
            return True
        time.sleep(1.0)
    return False

def capture_screenshot(url, output_path):
    print(f"Capturing screenshot for {url} -> {output_path}")
    chrome_cmd = [
        CHROME_PATH,
        "--headless=new",
        "--disable-gpu",
        "--window-size=1280,800",
        "--ignore-certificate-errors",
        f"--screenshot={output_path}",
        url
    ]
    try:
        subprocess.run(chrome_cmd, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, timeout=15)
        if os.path.exists(output_path):
            print(f"  Saved: {os.path.basename(output_path)}")
            return True
    except Exception as e:
        print(f"  Error capturing screenshot: {e}")
    return False

def clean_project_name(csproj_path):
    parts = csproj_path.split(os.sep)
    for part in parts:
        if part.startswith("Proje "):
            return part
    return os.path.splitext(os.path.basename(csproj_path))[0]

def main():
    print("=== ASP.NET CORE PORTFOLIO SCREENSHOT AUTOMATION ===")
    
    # 1. Kill existing dotnet processes to prevent port conflicts
    kill_existing_dotnet()
    
    # 2. Discover all web projects
    web_projects = find_web_projects()
    print(f"Found {len(web_projects)} web projects:")
    for wp in web_projects:
        print(f"  * {wp}")
    
    for csproj in web_projects:
        proj_name = clean_project_name(csproj)
        csproj_dir = os.path.dirname(csproj)
        subproj_name = os.path.splitext(os.path.basename(csproj))[0]
        
        url = parse_url_from_launch_settings(csproj_dir)
        if not url:
            url = "http://localhost:5000"
        url = url.replace("0.0.0.0", "localhost")
            
        print(f"\n==================================================")
        print(f"PROJECT: {proj_name} ({subproj_name})")
        print(f"BASE URL: {url}")
        print(f"==================================================")
        
        # Determine paths to capture
        routes_to_capture = []
        is_api = "api" in subproj_name.lower() or "api" in proj_name.lower() or "otomati" in subproj_name.lower()
        
        if is_api:
            # It's an API, capture Scalar and Swagger docs
            routes_to_capture = [
                "/scalar/v1",
                "/swagger/index.html",
                "/"
            ]
        else:
            # MVC project, discover views
            routes_to_capture = find_mvc_routes(csproj_dir)
            
        print("Routes to capture:")
        for r in routes_to_capture:
            print(f"  - {url}{r}")
            
        # Run dotnet in the foreground (direct output to console)
        print(f"\nRunning command: dotnet run --project \"{csproj}\"")
        process = None
        try:
            # On Windows we use CREATE_NEW_PROCESS_GROUP to cleanly taskkill it and its children
            process = subprocess.Popen(
                ["dotnet", "run", "--project", csproj],
                stdout=None,  # Output to foreground
                stderr=None,  # Output to foreground
                creationflags=subprocess.CREATE_NEW_PROCESS_GROUP if sys.platform == "win32" else 0
            )
            
            # Wait for port to open
            if wait_for_server(url, timeout=WAIT_TIMEOUT):
                # Wait 4 extra seconds for app compilation / DB migrations
                time.sleep(4.0)
                
                # Create project-specific directory for screenshots
                proj_screenshot_dir = os.path.join(OUTPUT_DIR, re.sub(r'[\\/*?:"<>|]', "", proj_name).strip())
                os.makedirs(proj_screenshot_dir, exist_ok=True)
                
                # Take screenshots
                for route in routes_to_capture:
                    full_url = f"{url}{route}"
                    safe_route_name = re.sub(r'[\\/*?:"<>|]', "_", route.strip("/")).replace(" ", "_")
                    if not safe_route_name:
                        safe_route_name = "Api_Root" if is_api else "Home"
                    
                    output_file = os.path.abspath(os.path.join(proj_screenshot_dir, f"{safe_route_name}.png"))
                    capture_screenshot(full_url, output_file)
            else:
                print(f"FAILED: Server for {proj_name} did not start in {WAIT_TIMEOUT} seconds.")
                
        except Exception as e:
            print(f"Error running project {proj_name}: {e}")
        finally:
            if process:
                print(f"\nStopping dotnet process for {proj_name}...")
                if sys.platform == "win32":
                    subprocess.run(["taskkill", "/F", "/T", "/PID", str(process.pid)], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                else:
                    process.terminate()
                    process.wait()
                print("Process stopped. Freeing up ports...")
                time.sleep(2.0)

    print("\nAll projects processed. Screenshots saved in the 'screenshots' directory.")

if __name__ == "__main__":
    main()
