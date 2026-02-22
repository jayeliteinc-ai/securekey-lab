let messages = [
{
role: "system",
content: "You are a friendly AI with long term memory."
}
];

function add(text) {
const div = document.createElement("div");
div.innerText = text;
document.getElementById("chat").appendChild(div);
}

async function send() {

const input = document.getElementById("input");

const text = input.value;

messages.push({
role: "user",
content: text
});

add("You: " + text);

input.value = "";

const res = await fetch("/api/chat", {
method: "POST",
headers: {
"Content-Type": "application/json"
},
body: JSON.stringify({ messages })
});

const data = await res.json();

add("AI: " + data.reply);

messages.push({
role: "assistant",
content: data.reply
});

}
