import discord
from discord.ext import commands
import openai
import os

TOKEN = "MTQxNDYyNzczODc1MjY1MTM0NA.G2jDK3.2IjI5l7iifgIf2Z2XQo87GOEH62P7TFX5-u7Wk"
OWNER_ID = 1390567860153221151

intents = discord.Intents.all()
bot = commands.Bot(command_prefix="!", intents=intents)

openai.api_key = "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"

active = True  # Jarvis on/off control

@bot.event
async def on_ready():
    print(f"Jarvis logged in as {bot.user}")

@bot.event
async def on_message(message):
    global active

    if message.author.bot:
        return

   
    if message.author.id == OWNER_ID and message.content.lower().startswith("jarvis"):
        command = message.content.lower().replace("jarvis", "").strip()

        if command == "":
            await message.channel.send("Yes Boss!")
        elif command == "band hoja":
            active = False
            await message.channel.send("Jarvis band ho gaya Boss!")
        elif command == "on hoja":
            active = True
            await message.channel.send("Jarvis ready hai Boss!")
        elif command.startswith("bolo "):
            text = command.replace("bolo ", "")
            await message.channel.send(text)
        elif command.startswith("soch "):
            if active:
                prompt = command.replace("soch ", "")
                response = openai.ChatCompletion.create(
                    model="gpt-3.5-turbo",
                    messages=[{"role": "user", "content": prompt}]
                )
                reply = response["choices"][0]["message"]["content"]
                await message.channel.send(reply)
        else:
            await message.channel.send("Samajh nahi aaya Boss!")

    await bot.process_commands(message)

bot.run(TOKEN)
