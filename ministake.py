from telegram import Update, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import ApplicationBuilder, CommandHandler, CallbackContext, CallbackQueryHandler
import random

# ========== CONFIG ==========
OWNER_ID = 7145581455
UPI_ID = "debduttapaulpyt@oksbi"
MANUAL_MINES = [(0,0), (0,1), (0,4), (1,1), (1,3), (2,0), (2,3), (3,1), (4,2), (4,4)]
GRID_SIZE = 5

# ========== DATA STORAGE ==========
users = {}  # {user_id: {plays_left, wallet, demo_done}}
active_games = {}

# ========== COMMANDS ==========

async def start(update: Update, context: CallbackContext):
    user_id = update.effective_user.id
    username = update.effective_user.username or "Unknown"
    if user_id not in users:
        users[user_id] = {"plays_left": 5, "wallet": 0, "demo_done": False}
        await update.message.reply_text(f"Welcome @{username}!\n\nYou have been given 5 free demo plays. Type /play to begin Mines.")
    else:
        await update.message.reply_text(f"Welcome back @{username}! You have {users[user_id]['plays_left']} plays left.")

async def balance(update: Update, context: CallbackContext):
    user_id = update.effective_user.id
    if user_id not in users:
        await update.message.reply_text("Please type /start to register first.")
        return
    b = users[user_id]['wallet']
    await update.message.reply_text(f"Your wallet balance: ₹{b}")

async def play(update: Update, context: CallbackContext):
    user_id = update.effective_user.id
    if user_id not in users:
        await update.message.reply_text("Type /start first.")
        return

    data = users[user_id]
    if data["plays_left"] > 0:
        data["plays_left"] -= 1
        await start_mines_game(update, context, demo=True)
    elif data["wallet"] >= 20:
        data["wallet"] -= 20
        await start_mines_game(update, context, demo=False)
    else:
        await update.message.reply_text(f"You're out of demo plays. Please deposit at least ₹200 to play.\nSend payment to: {UPI_ID}\nThen type /utr <your UTR number>")

async def utr_handler(update: Update, context: CallbackContext):
    user_id = update.effective_user.id
    if user_id not in users:
        await update.message.reply_text("Please type /start to register first.")
        return

    if user_id != OWNER_ID:
        await update.message.reply_text("Thanks! Your UTR has been received. Admin will verify and update your balance.")
        return

    try:
        target_id = int(context.args[0])
        amount = int(context.args[1])
        if target_id in users:
            users[target_id]['wallet'] += amount
            await update.message.reply_text(f"User {target_id} wallet credited with ₹{amount}.")
        else:
            await update.message.reply_text("Invalid user ID.")
    except:
        await update.message.reply_text("Usage: /utr <user_id> <amount>")

# ========== MINES GAME ==========

async def start_mines_game(update: Update, context: CallbackContext, demo=True):
    user_id = update.effective_user.id
    grid_buttons = []
    for i in range(GRID_SIZE):
        row = []
        for j in range(GRID_SIZE):
            row.append(InlineKeyboardButton("⬜", callback_data=f"tile_{i}_{j}_{demo}"))
        grid_buttons.append(row)

    reply_markup = InlineKeyboardMarkup(grid_buttons)
    active_games[user_id] = {"demo": demo, "clicked": [], "ended": False}
    await update.message.reply_text("🎮 Select a tile:", reply_markup=reply_markup)

async def handle_tile(update: Update, context: CallbackContext):
    query = update.callback_query
    await query.answer()
    user_id = query.from_user.id

    if user_id not in active_games or active_games[user_id]['ended']:
        await query.edit_message_text("❌ No active game. Type /play to start again.")
        return

    data = query.data.split("_")
    i, j = int(data[1]), int(data[2])
    demo = data[3] == "True"

    mines = random.sample([(x, y) for x in range(GRID_SIZE) for y in range(GRID_SIZE)], 5) if demo else MANUAL_MINES

    if (i, j) in active_games[user_id]['clicked']:
        return

    active_games[user_id]['clicked'].append((i, j))

    if (i, j) in mines:
        active_games[user_id]['ended'] = True
        await query.edit_message_text(f"💥 BOOM! You hit a mine at ({i+1}, {j+1}). Game over.")
    else:
        if len(active_games[user_id]['clicked']) >= (GRID_SIZE * GRID_SIZE - len(mines)):
            await query.edit_message_text("🎉 You cleared the board! You win!")
        else:
            await update_tile_board(query, user_id, mines)

async def update_tile_board(query, user_id, mines):
    grid_buttons = []
    for i in range(GRID_SIZE):
        row = []
        for j in range(GRID_SIZE):
            if (i, j) in active_games[user_id]['clicked']:
                row.append(InlineKeyboardButton("💎", callback_data="done"))
            else:
                row.append(InlineKeyboardButton("⬜", callback_data=f"tile_{i}_{j}_False"))
        grid_buttons.append(row)

    reply_markup = InlineKeyboardMarkup(grid_buttons)
    await query.edit_message_reply_markup(reply_markup=reply_markup)

# ========== MAIN ==========

async def main():
    app = ApplicationBuilder().token("8402851029:AAFVoqBbLo9z7SyoR9SFH62MNIY9qvwOPDQ").build()

    app.add_handler(CommandHandler("start", start))
    app.add_handler(CommandHandler("balance", balance))
    app.add_handler(CommandHandler("play", play))
    app.add_handler(CommandHandler("utr", utr_handler))
    app.add_handler(CallbackQueryHandler(handle_tile, pattern=r"tile_.*"))

    print("Bot is running...")
    await app.run_polling()

import asyncio
if __name__ == '__main__':
    asyncio.run(main())
