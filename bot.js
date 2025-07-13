const TelegramBot = require('node-telegram-bot-api');
const token = '7639926036:AAHtvhb56f811EBhhNNmcGi7b4mdfuXUnuI'; // replace this
const bot = new TelegramBot(token, { polling: true });

bot.onText(/\/start/, (msg) => {
  bot.sendMessage(msg.chat.id, "Bot is active, Boss!");
});
