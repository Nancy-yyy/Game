from fastapi import FastAPI
from pydantic import BaseModel
import os
import sys
from dotenv import load_dotenv
from pathlib import Path

from generate_answer import (
    call_gemini_chat,
    build_system_prompt,
    build_user_prompt
)

BASE_DIR = Path(__file__).resolve().parent.parent
load_dotenv(BASE_DIR / ".env")

app = FastAPI(title="UniMan AI Tutor Server")

class QueryRequest(BaseModel):
    query: str
    case_id: str = "case2"  # 預設為 case2，可傳入 case1 / case2 / case3

# 1. 補上心跳檢測端點，讓 Unity 的 ServerLauncher 不會超時報錯
@app.get("/health")
def health_check():
    return {"status": "ok"}

# 2. 核心問答端點
@app.post("/ask")
def ask(req: QueryRequest):
    try:
        print(f"[DEBUG] 收到關卡 [{req.case_id}] 請求: {req.query}", file=sys.stderr)
        
        # 讀取共通規則與對應關卡的知識庫文字檔
        data_dir = BASE_DIR / "data"
        rules_path = data_dir / "system_rules.txt"
        case_file_map = {
            "case1": data_dir / "case1_transport.txt",
            "case2": data_dir / "case2_textbook.txt",
            "case3": data_dir / "case3_space.txt"
        }
        case_path = case_file_map.get(req.case_id, data_dir / "case2_textbook.txt")
        
        system_rules = rules_path.read_text(encoding="utf-8") if rules_path.exists() else ""
        case_context = case_path.read_text(encoding="utf-8") if case_path.exists() else ""
        
        system_prompt = build_system_prompt(system_rules, case_context)
        user_prompt = build_user_prompt(req.query)
        
        # 呼叫 Gemini
        answer_text = call_gemini_chat(
            api_key=os.getenv("GEMINI_API_KEY"),
            system_prompt=system_prompt,
            user_prompt=user_prompt
        )
        
        return {
            "question": req.query,
            "case_id": req.case_id,
            "short_answer": answer_text
        }
        
    except Exception as e:
        import traceback
        traceback.print_exc()
        return {"error": str(e)}