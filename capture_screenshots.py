import os
import subprocess
import time
import json
import socket
import sys
import re
from selenium import webdriver
from selenium.webdriver.common.by import By

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

def capture_project_screenshots_selenium(url, proj_name, routes_to_capture, proj_screenshot_dir, is_api):
    options = webdriver.ChromeOptions()
    options.add_argument('--headless=new')
    options.add_argument('--disable-gpu')
    options.add_argument('--window-size=1280,800')
    options.add_argument('--ignore-certificate-errors')
    if os.path.exists(CHROME_PATH):
        options.binary_location = CHROME_PATH
        
    driver = webdriver.Chrome(options=options)
    
    try:
        # Determine if authentication is needed
        is_auth_project = any("login" in r.lower() for r in routes_to_capture)
        needs_login = False
        login_url = None
        
        if not is_api and is_auth_project:
            needs_login = True
            login_route = next((r for r in routes_to_capture if "login" in r.lower()), "/Account/Login")
            login_url = f"{url}{login_route}"
            print(f"Project requires authentication. Login URL: {login_url}")

        routes_to_capture_now = []
        authenticated_routes = []

        # Determine which routes can be captured logged out vs logged in
        if needs_login:
            for route in routes_to_capture:
                r_lower = route.lower()
                # 1. Auth-specific pages (login, register, access denied) MUST be captured logged out
                if "login" in r_lower or "register" in r_lower or "accessdenied" in r_lower:
                    routes_to_capture_now.append(route)
                else:
                    # 2. Check if a route redirects to login when logged out
                    full_url = f"{url}{route}"
                    try:
                        driver.get(full_url)
                        time.sleep(1.0)
                        curr_url_lower = driver.current_url.lower()
                        if "login" in curr_url_lower or "accessdenied" in curr_url_lower or "signin" in curr_url_lower:
                            # It redirected to login/auth page! So it requires authentication
                            authenticated_routes.append(route)
                        else:
                            # It did not redirect, meaning it's a public page. Capture it now.
                            routes_to_capture_now.append(route)
                    except Exception as e:
                        # Fallback: if check fails, assume it requires auth if it's admin or similar, or just capture now
                        if "admin" in r_lower:
                            authenticated_routes.append(route)
                        else:
                            routes_to_capture_now.append(route)
        else:
            routes_to_capture_now = routes_to_capture

        # Step 1: Capture unauthenticated and public pages
        for route in routes_to_capture_now:
            full_url = f"{url}{route}"
            safe_route_name = re.sub(r'[\\/*?:"<>|]', "_", route.strip("/")).replace(" ", "_")
            if not safe_route_name:
                safe_route_name = "Home"
                
            output_file = os.path.abspath(os.path.join(proj_screenshot_dir, f"{safe_route_name}.png"))
            print(f"Capturing unauthenticated route: {full_url} -> {output_file}")
            try:
                driver.get(full_url)
                time.sleep(2.0)
                driver.save_screenshot(output_file)
                print(f"  Saved: {os.path.basename(output_file)}")
            except Exception as e:
                print(f"  Error capturing route {route}: {e}")

        # Step 2: Perform Login/Registration if needed
        if needs_login and authenticated_routes:
            # 1. Registration
            register_route = next((r for r in routes_to_capture if "register" in r.lower()), "/Account/Register")
            register_url = f"{url}{register_route}"
            print(f"Attempting user registration at: {register_url}")
            try:
                driver.get(register_url)
                time.sleep(2.5)
                
                if "register" in driver.current_url.lower():
                    username_field = None
                    for name in ["Username", "username", "Email", "email"]:
                        try:
                            username_field = driver.find_element(By.NAME, name)
                            if username_field:
                                break
                        except:
                            pass
                    
                    if not username_field:
                        for selector in ["#username", "#Username", "input[type='text']"]:
                            try:
                                username_field = driver.find_element(By.CSS_SELECTOR, selector)
                                if username_field:
                                    break
                            except:
                                pass
                                
                    password_inputs = driver.find_elements(By.CSS_SELECTOR, "input[type='password']")
                    
                    u_val = "admin"
                    e_val = "admin@disaccord.com" if "discord" in proj_name.lower() else "admin@example.com"
                    if "school" in proj_name.lower() or "okul" in proj_name.lower():
                        p_val = "123456"
                    elif "discord" in proj_name.lower():
                        p_val = "Admin123!"
                    else:
                        p_val = "AdminPassword123!"
                    
                    if username_field:
                        username_field.clear()
                        if "email" in (username_field.get_attribute("name") or "").lower():
                            username_field.send_keys(e_val)
                        else:
                            username_field.send_keys(u_val)
                            
                    email_field = None
                    try:
                        email_field = driver.find_element(By.NAME, "Email")
                    except:
                        try:
                            email_field = driver.find_element(By.NAME, "email")
                        except:
                            pass
                    if email_field and email_field != username_field:
                        email_field.clear()
                        email_field.send_keys(e_val)
                        
                    for pf in password_inputs:
                        pf.clear()
                        pf.send_keys(p_val)
                        
                    submit_btn = None
                    for selector in ["button[type='submit']", "input[type='submit']", ".btn-submit", ".btn-primary"]:
                        try:
                            submit_btn = driver.find_element(By.CSS_SELECTOR, selector)
                            if submit_btn:
                                break
                        except:
                            pass
                    if submit_btn:
                        submit_btn.click()
                        print("Registration form submitted.")
                        time.sleep(3.0)
            except Exception as reg_ex:
                print(f"Registration step skipped or failed: {reg_ex}")
                
            # 2. Login
            print(f"Navigating to login page: {login_url}")
            driver.get(login_url)
            time.sleep(2.5)
            
            try:
                username_field = None
                for name in ["Username", "username", "Email", "email"]:
                    try:
                        username_field = driver.find_element(By.NAME, name)
                        if username_field:
                            break
                    except:
                        pass
                
                if not username_field:
                    for selector in ["#username", "#Username", "input[type='text']", "input[type='email']"]:
                        try:
                            username_field = driver.find_element(By.CSS_SELECTOR, selector)
                            if username_field:
                                break
                        except:
                            pass
                            
                password_field = None
                try:
                    password_field = driver.find_element(By.NAME, "Password")
                except:
                    try:
                        password_field = driver.find_element(By.NAME, "password")
                    except:
                        try:
                            password_field = driver.find_element(By.CSS_SELECTOR, "input[type='password']")
                        except:
                            pass
                            
                u_val = "admin"
                e_val = "admin@disaccord.com" if "discord" in proj_name.lower() else "admin@example.com"
                if "school" in proj_name.lower() or "okul" in proj_name.lower():
                    p_val = "123456"
                elif "discord" in proj_name.lower():
                    p_val = "Admin123!"
                else:
                    p_val = "AdminPassword123!"
                
                if username_field:
                    username_field.clear()
                    if "email" in (username_field.get_attribute("name") or "").lower() or "email" in (username_field.get_attribute("id") or "").lower():
                        username_field.send_keys(e_val)
                    else:
                        username_field.send_keys(u_val)
                        
                if password_field:
                    password_field.clear()
                    password_field.send_keys(p_val)
                    
                submit_btn = None
                for selector in ["button[type='submit']", "input[type='submit']", ".btn-submit", ".btn-primary"]:
                    try:
                        submit_btn = driver.find_element(By.CSS_SELECTOR, selector)
                        if submit_btn:
                            break
                    except:
                        pass
                if submit_btn:
                    submit_btn.click()
                    print("Login form submitted.")
                    time.sleep(3.0)
            except Exception as log_ex:
                print(f"Login form submit failed: {log_ex}")

            # Step 3: Capture authenticated routes
            for route in authenticated_routes:
                full_url = f"{url}{route}"
                safe_route_name = re.sub(r'[\\/*?:"<>|]', "_", route.strip("/")).replace(" ", "_")
                if not safe_route_name:
                    safe_route_name = "Home"
                output_file = os.path.abspath(os.path.join(proj_screenshot_dir, f"{safe_route_name}.png"))
                print(f"Capturing authenticated route: {full_url} -> {output_file}")
                try:
                    driver.get(full_url)
                    time.sleep(2.0)
                    driver.save_screenshot(output_file)
                    print(f"  Saved: {os.path.basename(output_file)}")
                except Exception as e:
                    print(f"  Error capturing route {route}: {e}")
    finally:
        driver.quit()

def clean_project_name(csproj_path):
    parts = csproj_path.split(os.sep)
    for part in parts:
        if part.startswith("Proje "):
            return part
    return os.path.splitext(os.path.basename(csproj_path))[0]

def main():
    print("=== ASP.NET CORE PORTFOLIO SCREENSHOT AUTOMATION ===")
    
    # Filter by command-line arguments if provided
    filter_proj = None
    if len(sys.argv) > 1:
        filter_proj = sys.argv[1].lower()
        print(f"Filtering projects matching: '{filter_proj}'")
    
    # 1. Kill existing dotnet processes to prevent port conflicts
    kill_existing_dotnet()
    
    # 2. Discover all web projects
    web_projects = find_web_projects()
    print(f"Found {len(web_projects)} web projects:")
    for wp in web_projects:
        print(f"  * {wp}")
    
    for csproj in web_projects:
        proj_name = clean_project_name(csproj)
        if filter_proj and filter_proj not in proj_name.lower():
            continue
            
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
        
        # Check Program.cs for Swagger/Scalar
        has_scalar = False
        has_swagger = False
        program_cs = os.path.join(csproj_dir, "Program.cs")
        if os.path.exists(program_cs):
            try:
                with open(program_cs, "r", encoding="utf-8-sig") as f:
                    content = f.read()
                    if "MapScalarApiReference" in content:
                        has_scalar = True
                    if "UseSwagger" in content:
                        has_swagger = True
            except Exception as e:
                print(f"Error reading Program.cs for {proj_name}: {e}")

        if is_api:
            # It's an API, capture Scalar and Swagger docs if configured
            if has_scalar:
                routes_to_capture.append("/scalar/v1")
            if has_swagger:
                routes_to_capture.append("/swagger/index.html")
            
            # Check if there is an MVC sibling project in the same folder
            has_mvc_sibling = False
            parent_dir = os.path.dirname(csproj_dir)
            if os.path.exists(parent_dir):
                for root, dirs, files in os.walk(parent_dir):
                    if "bin" in root or "obj" in root:
                        continue
                    if any(f.endswith(".csproj") for f in files) and root != csproj_dir:
                        # Found another csproj in the same Proje folder
                        if "api" not in os.path.basename(root).lower():
                            has_mvc_sibling = True
                            break
            
            if not has_mvc_sibling:
                routes_to_capture.append("/")
        else:
            # MVC project, discover views
            routes_to_capture = find_mvc_routes(csproj_dir)
            
        print("Routes to capture:")
        for r in routes_to_capture:
            print(f"  - {url}{r}")
            
        # Check if there is an API sibling project to start first in the background
        api_sibling_process = None
        if not is_api:
            parent_dir = os.path.dirname(csproj_dir)
            if os.path.exists(parent_dir):
                for root, dirs, files in os.walk(parent_dir):
                    if "bin" in root or "obj" in root:
                        continue
                    for f in files:
                        if f.endswith(".csproj") and "api" in f.lower() and os.path.join(root, f) != csproj:
                            api_csproj = os.path.join(root, f)
                            api_url = parse_url_from_launch_settings(root)
                            if not api_url:
                                api_url = "http://localhost:5000"
                            api_url = api_url.replace("0.0.0.0", "localhost")
                            
                            print(f"\nDetected API sibling: {f} at {api_url}. Starting in background...")
                            try:
                                api_sibling_process = subprocess.Popen(
                                    ["dotnet", "run", "--project", api_csproj],
                                    stdout=subprocess.DEVNULL,
                                    stderr=subprocess.DEVNULL,
                                    creationflags=subprocess.CREATE_NEW_PROCESS_GROUP if sys.platform == "win32" else 0
                                )
                                # Wait for API server to respond
                                if wait_for_server(api_url, timeout=WAIT_TIMEOUT):
                                    print(f"API sibling is up and running.")
                                    time.sleep(2.0)
                                else:
                                    print(f"WARNING: API sibling at {api_url} did not start.")
                            except Exception as api_err:
                                print(f"Error starting API sibling: {api_err}")
                            break

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
                
                # Capture screenshots using Selenium
                capture_project_screenshots_selenium(url, proj_name, routes_to_capture, proj_screenshot_dir, is_api)
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
            if api_sibling_process:
                print(f"Stopping API sibling process...")
                if sys.platform == "win32":
                    subprocess.run(["taskkill", "/F", "/T", "/PID", str(api_sibling_process.pid)], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
                else:
                    api_sibling_process.terminate()
                    api_sibling_process.wait()
                time.sleep(1.0)

    print("\nAll projects processed. Screenshots saved in the 'screenshots' directory.")

if __name__ == "__main__":
    main()
