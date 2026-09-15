import os
import sys
from google import genai
from google.genai import types

def build_system_prompt(system_rules: str, case_context: str) -> str:
    return f"""
{system_rules}

【當前關卡背景與共享經濟知識】
{case_context}

【防暴雷規則】
上方提供的內容代表「玩家目前已經知道、或目前允許使用的資訊」。
你只能根據這些資訊回答與當前劇情有關的問題。
禁止推測、補充或提前透露玩家尚未遇到的人物、價格、方案、事件、答案或後續劇情。
即使你從一般知識推測得到後續內容，也不得提前告知玩家。
若玩家詢問與遊戲無關的日常問題，可以簡短以鳥鳥角色回答，但不要自行帶入未揭露的關卡內容。


【回覆準則（非常重要）】
1. 嚴禁直接透露選擇題或排序題答案。
2. 保持調皮、聰明的鳥鳥口吻（帶有啾～或嘿嘿）。
3. ⭐【字數極度精簡】：回覆嚴格限制在「1 到 2 句話以內」（總字數不超過 70 字）！
4. 採用蘇格拉底式提問，直接針對玩家痛點引導思考（例如引導思考「四個月後還會看嗎？」或「你真的需要永久擁有它嗎？」）。
5. 禁止輸出任何廢話、長篇大論或空行分段。
"""

def build_user_prompt(query: str) -> str:
    return f"玩家向你請教問題：「{query}」\n請以鳥鳥身分用 1~2 句話進行極簡反思性引導："

def call_gemini_chat(api_key: str, system_prompt: str, user_prompt: str) -> str:
    clean_key = api_key.strip().strip('"').strip("'") if api_key else ""
    if not clean_key:
        return "鳥鳥忘記帶 API 金鑰鑰匙了，請在 .env 檔案設定 GEMINI_API_KEY 喔！啾～"

    try:
        client = genai.Client(api_key=clean_key)

        chat = client.chats.create(
            model="gemini-3.5-flash-lite",
            config=types.GenerateContentConfig(
                system_instruction=system_prompt,
                temperature=0.3,
                max_output_tokens=100  # ⭐ 調小 Token 上限，強制它簡短精練
            )
        )
        
        response = chat.send_message(user_prompt)
        reply_text = response.text.strip()
        print(f"\n[AI 極簡回覆]:\n{reply_text}\n", file=sys.stderr)
        return reply_text
    except Exception as e:
        print(f"[Gemini Error] {e}", file=sys.stderr)
        return f"鳥鳥腦袋打結了...（{str(e)}）"