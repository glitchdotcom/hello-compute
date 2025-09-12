from flask import Flask
import threading

app = Flask('')

@app.route('/')
def home():
    return "✅ البوت شغال يا معلم!"

def run():
    app.run(host='0.0.0.0', port=3000)

threading.Thread(target=run).start()
