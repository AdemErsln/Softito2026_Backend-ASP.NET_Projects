import os
import subprocess
import time
import sys
import socket
from selenium import webdriver
from selenium.webdriver.common.by import By

CHROME_PATH = r"C:\Program Files\Google\Chrome\Application\chrome.exe"
SCREENSHOT_DIR = r"c:\Users\adem_\Documents\Softito2026_Backend-ASP.NET_Projects\screenshots\Proje 6 - NezLojistik"
os.makedirs(SCREENSHOT_DIR, exist_ok=True)

def wait_for_server(port, timeout=60):
    print(f"Waiting for port {port} to open...")
    start_time = time.time()
    while time.time() - start_time < timeout:
        try:
            with socket.create_connection(("localhost", port), timeout=1.0):
                print(f"Port {port} is open!")
                return True
        except (socket.timeout, ConnectionRefusedError):
            time.sleep(1.0)
    print(f"Timeout waiting for port {port}.")
    return False

def start_backend():
    print("Starting API Backend (NezLojstik)...")
    api_proc = subprocess.Popen(
        ["dotnet", "run", "--project", r".\Proje 6 - NezLojistik\NezLojstik\NezLojstikApi.csproj"],
        stdout=subprocess.DEVNULL,
        stderr=subprocess.DEVNULL
    )
    return api_proc

def start_frontend():
    print("Starting MVC Frontend (NezLojistikMvc)...")
    log_file = open("scratch/mvc_output.log", "w", encoding="utf-8")
    mvc_proc = subprocess.Popen(
        ["dotnet", "run", "--project", r".\Proje 6 - NezLojistik\NezLojistikMvc\NezLojistikMvc.csproj"],
        stdout=log_file,
        stderr=log_file
    )
    return mvc_proc, log_file

def get_driver():
    options = webdriver.ChromeOptions()
    options.add_argument('--headless=new')
    options.add_argument('--disable-gpu')
    options.add_argument('--window-size=1280,800')
    options.add_argument('--ignore-certificate-errors')
    if os.path.exists(CHROME_PATH):
        options.binary_location = CHROME_PATH
    return webdriver.Chrome(options=options)

def main():
    api_proc = start_backend()
    mvc_proc, mvc_log = start_frontend()
    
    # Wait for both ports
    api_ready = wait_for_server(5046)
    mvc_ready = wait_for_server(5289)
    
    if not (api_ready and mvc_ready):
        print("Error: Servers did not start in time.")
        api_proc.terminate()
        mvc_proc.terminate()
        mvc_log.close()
        sys.exit(1)
        
    # --- USER SESSION ---
    print("Starting User Session...")
    driver = get_driver()
    try:
        # 1. Capture Swagger (API)
        print("Capturing API Swagger...")
        driver.get("http://localhost:5046/swagger/index.html")
        time.sleep(3.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "swagger_index.html.png"))
        print("Captured swagger_index.html.png")
        
        # 2. Capture MVC Home
        print("Capturing MVC Home Page...")
        driver.get("http://localhost:5289/")
        time.sleep(3.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Home.png"))
        print("Captured Home.png")
        
        # 3. Capture MVC Privacy
        print("Capturing MVC Privacy Page...")
        driver.get("http://localhost:5289/Home/Privacy")
        time.sleep(2.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Home_Privacy.png"))
        print("Captured Home_Privacy.png")
        
        # 4. Capture Account Login Page (Logged out)
        print("Capturing Account Login Page (Logged out)...")
        driver.get("http://localhost:5289/Account/Login")
        time.sleep(2.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Account_Login.png"))
        print("Captured Account_Login.png")
        
        # 5. Capture Account Register Page
        print("Capturing Account Register Page...")
        driver.get("http://localhost:5289/Account/Register")
        time.sleep(2.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Account_Register.png"))
        print("Captured Account_Register.png")
        
        # 6. Log in as User
        print("Logging in as User (ahmet@nez.com)...")
        driver.get("http://localhost:5289/Account/Login")
        time.sleep(2.0)
        
        u_email = driver.find_element(By.NAME, "Email")
        u_pass = driver.find_element(By.NAME, "Password")
        u_email.clear()
        u_email.send_keys("ahmet@nez.com")
        u_pass.clear()
        u_pass.send_keys("123456")
        
        driver.find_element(By.CSS_SELECTOR, "button[type='submit']").click()
        time.sleep(3.0)
        print(f"User login redirect URL: {driver.current_url}")
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Account_Dashboard.png"))
        print("Captured Account_Dashboard.png")
    finally:
        driver.quit()

    # --- ADMIN SESSION ---
    print("\nStarting Admin Session...")
    driver = get_driver()
    try:
        # 7. Capture Admin Login Page (Logged out)
        print("Capturing Admin Login Page (Logged out)...")
        driver.get("http://localhost:5289/Admin/Login")
        time.sleep(2.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Admin_Login.png"))
        print("Captured Admin_Login.png")
        
        # 8. Log in as Admin
        print("Logging in as Admin (admin@nez.com)...")
        driver.get("http://localhost:5289/Admin/Login")
        time.sleep(2.0)
        
        a_email = driver.find_element(By.NAME, "Email")
        a_pass = driver.find_element(By.NAME, "Password")
        a_email.clear()
        a_email.send_keys("admin@nez.com")
        a_pass.clear()
        a_pass.send_keys("admin123")
        
        driver.find_element(By.CSS_SELECTOR, "button[type='submit']").click()
        time.sleep(3.0)
        print(f"Admin login redirect URL: {driver.current_url}")
        
        # Capture Admin Panel
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Admin_Panel.png"))
        print("Captured Admin_Panel.png")
        
        # 9. Capture Admin Depots
        print("Capturing Admin Depots...")
        driver.get("http://localhost:5289/Admin/Depots")
        time.sleep(3.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Admin_Depots.png"))
        print("Captured Admin_Depots.png")
        
        # 10. Capture Admin Drivers
        print("Capturing Admin Drivers...")
        driver.get("http://localhost:5289/Admin/Drivers")
        time.sleep(3.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Admin_Drivers.png"))
        print("Captured Admin_Drivers.png")
        
        # 11. Capture Admin Vehicles
        print("Capturing Admin Vehicles...")
        driver.get("http://localhost:5289/Admin/Vehicles")
        time.sleep(3.0)
        driver.save_screenshot(os.path.join(SCREENSHOT_DIR, "Admin_Vehicles.png"))
        print("Captured Admin_Vehicles.png")
        
    finally:
        driver.quit()
        print("Stopping background dotnet processes...")
        api_proc.terminate()
        mvc_proc.terminate()
        mvc_log.close()
        try:
            api_proc.wait(timeout=2.0)
            mvc_proc.wait(timeout=2.0)
        except:
            pass
        if sys.platform == "win32":
            subprocess.run(["taskkill", "/F", "/IM", "dotnet.exe"], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

if __name__ == "__main__":
    main()
