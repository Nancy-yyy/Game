@echo off
cd /d "%~dp0Assets\Scripts\AI\AI_Server"
C:\Users\ASUS\AppData\Local\Microsoft\WindowsApps\PythonSoftwareFoundation.Python.3.12_qbz5n2kfra8p0\python.exe -m uvicorn scripts.api_server:app --reload
pause