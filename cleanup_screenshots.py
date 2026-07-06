import os
import re
import json

OUTPUT_DIR = "screenshots"
PROJECTS_DIR = "."

def find_web_projects():
    web_projects = []
    for root, dirs, files in os.walk(PROJECTS_DIR):
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

def find_mvc_routes(project_dir):
    routes = ["/"]
    views_dir = os.path.join(project_dir, "Views")
    areas_dir = os.path.join(project_dir, "Areas")
    pages_dir = os.path.join(project_dir, "Pages")
    
    if os.path.exists(pages_dir):
        for root, dirs, files in os.walk(pages_dir):
            if "Shared" in root:
                continue
            for file in files:
                if file.endswith(".cshtml") and not file.startswith("_"):
                    rel_path = os.path.relpath(os.path.join(root, file), pages_dir)
                    rel_route = os.path.splitext(rel_path)[0].replace(os.sep, "/")
                    
                    if rel_route.lower() == "index":
                        continue
                    
                    if rel_route.lower().endswith("/index"):
                        route = "/" + rel_route[:-6]
                    else:
                        route = "/" + rel_route
                        
                    base_name = os.path.splitext(file)[0].lower()
                    if base_name in ["duzenle", "sil", "edit", "delete", "details", "update"]:
                        route += "/1"
                        
                    routes.append(route)
                    
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
                                        
    return sorted(list(set(routes)))

def clean_project_name(csproj_path):
    parts = csproj_path.split(os.sep)
    for part in parts:
        if part.startswith("Proje "):
            return part
    return os.path.splitext(os.path.basename(csproj_path))[0]

def main():
    print("=== STARTING SCREENSHOT CLEANUP ===")
    web_projects = find_web_projects()
    
    # Group expected files by screenshot directory
    dir_to_expected_files = {}
    
    for csproj in web_projects:
        proj_name = clean_project_name(csproj)
        csproj_dir = os.path.dirname(csproj)
        subproj_name = os.path.splitext(os.path.basename(csproj))[0]
        
        proj_screenshot_dir = os.path.abspath(os.path.join(OUTPUT_DIR, re.sub(r'[\\/*?:"<>|]', "", proj_name).strip()))
        
        is_api = "api" in subproj_name.lower() or "api" in proj_name.lower() or "otomati" in subproj_name.lower()
        if is_api:
            routes = ["/scalar/v1", "/swagger/index.html", "/"]
        else:
            routes = find_mvc_routes(csproj_dir)
            
        expected_files = set()
        for route in routes:
            safe_route_name = re.sub(r'[\\/*?:"<>|]', "_", route.strip("/")).replace(" ", "_")
            if not safe_route_name:
                safe_route_name = "Api_Root" if is_api else "Home"
            expected_files.add(f"{safe_route_name}.png")
            
        if proj_screenshot_dir not in dir_to_expected_files:
            dir_to_expected_files[proj_screenshot_dir] = set()
        dir_to_expected_files[proj_screenshot_dir].update(expected_files)
        
    for proj_screenshot_dir, expected_files in dir_to_expected_files.items():
        if not os.path.exists(proj_screenshot_dir):
            continue
            
        print(f"\nProject Directory: {os.path.basename(proj_screenshot_dir)}")
        print(f"Total expected files: {len(expected_files)}")
        
        # Scan and delete unexpected files
        for filename in os.listdir(proj_screenshot_dir):
            if filename.endswith(".png"):
                if filename not in expected_files:
                    file_path = os.path.join(proj_screenshot_dir, filename)
                    print(f"  Deleting stale/404 screenshot: {filename}")
                    try:
                        os.remove(file_path)
                    except Exception as e:
                        print(f"  Error deleting {filename}: {e}")
                        
    # Clean root-level loose images if any
    for filename in os.listdir(OUTPUT_DIR):
        if filename.endswith(".png") and os.path.isfile(os.path.join(OUTPUT_DIR, filename)):
            file_path = os.path.join(OUTPUT_DIR, filename)
            print(f"Deleting loose root screenshot: {filename}")
            try:
                os.remove(file_path)
            except Exception as e:
                print(f"Error: {e}")
                
    print("\n=== CLEANUP COMPLETED ===")

if __name__ == "__main__":
    main()
