const TelegramBot = require('node-telegram-bot-api');
const express = require('express');
const app = express();

// 1. ضع التوكن هنا
const TOKEN = "7868821401:AAEjomsl0i8GEfigrdwL1-MRSvhe8cqw_hk";
const bot = new TelegramBot(TOKEN, { polling: true });

// 2. عندما يرسل أحد /start
bot.onText(/\/start/, (msg) => {
  const chatId = msg.chat.id;
  bot.sendMessage(chatId, "🎮 **أهلاً! أنا بوتك!**\n\nإرسل /help للمساعدة.");
});

// 3. راوتر بسيط لموقعك
app.get("/", (req, res) => {
  res.send("مرحبًا! هذا موقعي 🏠");
});

// 4. تشغيل السيرفر
app.listen(3000, () => console.log("يعمل!"));
