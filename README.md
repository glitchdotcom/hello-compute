import time
import random
import os

def clear():
    os.system('cls' if os.name == 'nt' else 'clear')

clear()
print("🔐 Connecting to secure server...")
time.sleep(1.5)

print("📡 Establishing encrypted tunnel...")
time.sleep(2)

print("💻 Bypassing firewall...")
time.sleep(1)

print("🔓 Accessing system...")
time.sleep(2)

# شبیه در حال هک کردن بودن
for i in range(20):
    user = f"user_{random.randint(1000,9999)}"
    password = ''.join(random.choices("abcdefghijklmnopqrstuvwxyz1234567890", k=8))
    print(f"[+] Cracked credentials → Username: {user} | Password: {password}")
    time.sleep(0.2)

print("\n✅ System access granted.")
print("📁 Downloading sensitive data...")
time.sleep(2)

for i in range(0, 101, 10):
    print(f"⬇️ Downloading... {i}%")
    time.sleep(0.4)

print("\n🔥 Mission Complete.")
print("👻 Exiting without trace...")
time.sleep(1)
clear()
