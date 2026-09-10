import os
import sys
from pathlib import Path
from fastapi import FastAPI
from pydantic import BaseModel
from dotenv import load_dotenv

# 引用同目錄下的生成模組[cite: 1]
from scripts.generate_answer import (
    call_gemini_chat,
    build_system_prompt,
    build_user_prompt
)

BASE_DIR = Path(__file__).resolve().parent.parent
env_path = BASE_DIR / ".env"

# 印出除錯訊息
print(f"[DEBUG] 載入設定檔路徑: {env_path}", file=sys.stderr)
print(f"[DEBUG] 設定檔是否存在: {env_path.exists()}", file=sys.stderr)

load_dotenv(dotenv_path=env_path)

api_key = os.getenv("GEMINI_API_KEY")
if not api_key:
    print("[WARNING] 警告：GEMINI_API_KEY 讀取為空，請確認 .env 內容！", file=sys.stderr)
else:
    print(f"[SUCCESS] 成功讀取 API Key，長度: {len(api_key)} 字元", file=sys.stderr)

# FastAPI 實例[cite: 1]
app = FastAPI(title="UniMan AI Tutor Server")

class QueryRequest(BaseModel):
    query: str
    case_id: str = "case2"

@app.get("/health")
def health_check():
    return {"status": "ok"}

@app.post("/ask")
def ask(req: QueryRequest):
    try:
        data_dir = BASE_DIR / "data"

        # ⭐ 1. 容錯讀取規則檔 (優先讀取截圖中的 datasystem_rules.txt)
        rules_path = data_dir / "datasystem_rules.txt"
        if not rules_path.exists():
            rules_path = data_dir / "system_rules.txt"

        # ⭐ 2. 根據 Unity 傳入的 Scene 名稱智能匹配知識庫檔案
        case_id_lower = req.case_id.lower()
        if "case1" in case_id_lower:
            case_path = data_dir / "case1_transport.txt"
        elif "case3" in case_id_lower:
            case_path = data_dir / "case3_space.txt"
        else:
            # 包含 Case2 與 Ending 相關場景均以教科書案例為主
            case_path = data_dir / "case2_textbook.txt"

        # 讀取文字內容防呆
        system_rules = rules_path.read_text(encoding="utf-8") if rules_path.exists() else ""
        case_context = case_path.read_text(encoding="utf-8") if case_path.exists() else ""

        print(f"[INFO] 收到場景: {req.case_id} ➔ 匹配知識庫: {case_path.name}", file=sys.stderr)

        system_prompt = build_system_prompt(system_rules, case_context)
        user_prompt = build_user_prompt(req.query)

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
        return {
            "question": req.query,
            "case_id": req.case_id,
            "short_answer": f"鳥鳥腦袋暫時短路了...（{str(e)}）"
        }