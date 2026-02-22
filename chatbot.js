import OpenAI from "openai";
import readline from "readline";

const client = new OpenAI({ apiKey: "YOUR_API_KEY_HERE" });

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

let messages = [
  { role: "system", content: "You are a friendly AI companion." }
];

function ask() {
  rl.question("You: ", async (input) => {
    messages.push({ role: "user", content: input });

    const response = await client.chat.completions.create({
      model: "gpt-4o-mini",
      messages: messages,
    });

    const reply = response.choices[0].message.content;
    console.log("Bot:", reply);

    messages.push({ role: "assistant", content: reply });

    ask();
  });
}

ask();
