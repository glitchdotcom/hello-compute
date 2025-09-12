import express from "express";
import bodyParser from "body-parser";
import fetch from "node-fetch";

const app = express();
app.use(bodyParser.json());

const WHATSAPP_TOKEN = process.env.WHATSAPP_TOKEN;
const PHONE_NUMBER_ID = process.env.PHONE_NUMBER_ID;
const VERIFY_TOKEN = process.env.VERIFY_TOKEN || "dishwasher-secret";

// Queue
let queue = ["Alice", "Bob", "Charlie"];

// Verify webhook (Meta step)
app.get("/webhook", (req, res) => {
  const mode = req.query["hub.mode"];
  const token = req.query["hub.verify_token"];
  const challenge = req.query["hub.challenge"];

  if (mode && token && mode === "subscribe" && token === VERIFY_TOKEN) {
    res.status(200).send(challenge);
  } else {
    res.sendStatus(403);
  }
});

// Handle WhatsApp messages
app.post("/webhook", async (req, res) => {
  const msg = req.body.entry?.[0]?.changes?.[0]?.value?.messages?.[0];
  if (!msg) return res.sendStatus(200);

  const from = msg.from;
  const text = msg.text?.body?.trim() || "";

  let reply = "";

  if (text.toLowerCase() === "!queue") {
    reply = `Current queue: ${queue.join(" → ")}`;
  } else if (text.toLowerCase() === "!next") {
    const person = queue.shift();
    queue.push(person);
    reply = `Next up: ${person}. New queue: ${queue.join(" → ")}`;
  } else if (text.toLowerCase().startsWith("!punish")) {
    const parts = text.split(" ");
    const name = parts[1];
    const times = parseInt(parts[2] || "3");
    for (let i = 0; i < times; i++) queue.push(name);
    reply = `${name} punished with ${times} extra turns! New queue: ${queue.join(" → ")}`;
  } else if (text.toLowerCase().startsWith("!swap")) {
    const parts = text.split(" ");
    const a = parts[1], b = parts[2];
    const i = queue.indexOf(a), j = queue.indexOf(b);
    if (i > -1 && j > -1) [queue[i], queue[j]] = [queue[j], queue[i]];
    reply = `Swapped ${a} and ${b}. Queue: ${queue.join(" → ")}`;
  } else {
    reply = "Commands:\n!queue\n!next\n!punish NAME [times]\n!swap NAME1 NAME2";
  }

  await fetch(`https://graph.facebook.com/v21.0/${PHONE_NUMBER_ID}/messages`, {
    method: "POST",
    headers: {
      "Authorization": `Bearer ${WHATSAPP_TOKEN}`,
      "Content-Type": "application/json",
    },
    body: JSON.stringify({
      messaging_product: "whatsapp",
      to: from,
      text: { body: reply },
    }),
  });

  res.sendStatus(200);
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`✅ Dishwasher bot running on port ${PORT}`));
