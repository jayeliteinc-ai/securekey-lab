import OpenAI from "openai";

const client = new OpenAI({
  apiKey: process.env.OPENAI_API_KEY,
});

export default async function handler(req, res) {

  if (req.method !== "POST") {
    return res.status(405).send("Method not allowed");
  }

  const { messages } = req.body;

  const completion = await client.chat.completions.create({
    model: "gpt-4o-mini",
    messages: messages,
  });

  res.status(200).json({
    reply: completion.choices[0].message.content,
  });
}
