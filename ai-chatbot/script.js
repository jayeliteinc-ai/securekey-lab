const API_KEY = "YOUR_OPENAI_API_KEY";

let messages = [
  {
    role: "system",
    content: "You are a friendly AI companion with memory and personality."
  }
];

function addMessage(text, sender) {
  const chat = document.getElementById("chat");

  const div = document.createElement("div");
  div.className = "message " + sender;
  div.innerText = sender + ": " + text;

  chat.appendChild(div);
  chat.scrollTop = chat.scrollHeight;
}

async function sendMessage() {
  const input = document.getElementById("input");
  const text = input.value;

  if (!text) return;

  addMessage(text, "user");

  messages.push({
    role: "user",
    content: text
  });

  input.value = "";

  const response = await fetch("https://api.openai.com/v1/chat/completions", {
    method: "POST",
    headers: {
      "Authorization": "Bearer " + API_KEY,
      "Content-Type": "application/json"
    },
    body: JSON.stringify({
      model: "gpt-4o-mini",
      messages: messages
    })
  });

  const data = await response.json();

  const reply = data.choices[0].message.content;

  addMessage(reply, "bot");

  messages.push({
    role: "assistant",
    content: reply
  });
}
