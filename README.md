## SecureKey Lab

SecureKey Lab is an educational cybersecurity platform for Android that teaches
ethical hacking, mobile security, and cyber defense through simulations,
labs, and capture-the-flag challenges.

🚫 No exploits
🚫 No password cracking
✅ Legal, ethical, Play-Store-safe

## OpenAI SDK

For any AI-assisted in-app explanations (for example: safe hints, concept
walkthroughs, or lab debriefs), use the official OpenAI SDK from a backend
service instead of directly from the Android client.

### Why backend-only

- Keeps API keys off user devices.
- Allows request/response filtering and abuse prevention.
- Supports centralized logging and moderation.

### Python backend example

```bash
pip install openai
```

```python
from openai import OpenAI

client = OpenAI()

response = client.responses.create(
    model="gpt-4.1-mini",
    input="Explain SQL injection defensively in 3 bullets for beginners.",
)

print(response.output_text)
```

### Security notes

- Store `OPENAI_API_KEY` in server-side secrets management.
- Never bundle keys in APKs or client-side source.
- Add output guardrails to keep all content legal/ethical and educational.
