import os
from openai import OpenAI

DEFAULT_GEMINI_BASE_URL = "https://generativelanguage.googleapis.com/v1beta/openai/"
DEFAULT_GEMINI_MODEL = "gemini-1.5-flash"  # 或 gemini-2.5-flash

def build_system_prompt(system_rules: str, case_context: str) -> str:
    return f"""
{system_rules}

【當前關卡背景與共享經濟知識】
{case_context}

【回覆準則】
1. 嚴禁直接透露選擇題或排序題的正確答案（例如不能說「選C」、「順序是ABCD」）。
2. 用親切、調皮、聰明的鳥鳥口吻（帶有啾～或嘿嘿）。
3. 採用蘇格拉底式提問，每次引導 1~2 句話，引導玩家思考「所有權與使用權」或「自身真實需求」。
"""

def build_user_prompt(query: str) -> str:
    return f"玩家向你請教問題：「{query}」\n請以鳥鳥的身分進行反思性引導回答："

def call_gemini_chat(api_key: str, system_prompt: str, user_prompt: str) -> str:
    client = OpenAI(
        api_key=api_key,
        base_url=DEFAULT_GEMINI_BASE_URL
    )
    response = client.chat.completions.create(
        model=DEFAULT_GEMINI_MODEL,
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        temperature=0.4,
        max_tokens=250
    )
    return response.choices[0].message.content.strip()