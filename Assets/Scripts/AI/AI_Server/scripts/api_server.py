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
    
    # ======================================
    # AI 防暴雷：接收 Unity 傳來的劇情進度
    story_step: str = "unknown"
    # ======================================
 
 
# ======================================
# AI 防暴雷：Case 1 各劇情進度允許使用的資訊
# ======================================
CASE1_REVEALED_INFO = {

    "case1_car_started": """
玩家目前正在觀察共乘情境中的車輛與既有資源。
玩家正在學習辨認哪些資源或容量目前沒有被充分利用。
可以用問題引導玩家觀察「原本已存在」以及「目前是否正在被使用」。
禁止指出車內互動題的正確物件、正確位置或答案。
""",

    "case1_idle_capacity_completed": """
玩家已完成車內閒置容量辨識。
玩家已理解：原本已存在，但目前沒有被充分利用的座位或載物容量，可能具有重新配置與利用的價值。
玩家即將進入下一階段。
不要提前透露平台媒合活動的操作方式、排序答案或後續題目答案。
""",

    "case1_matching_started": """
玩家已完成車內閒置容量辨識，目前正在進行平台媒合相關活動。
玩家可以思考：資源提供者、需求者與平台之間如何形成媒合。
只能提供概念性提示。
禁止透露排序題的正確順序。
禁止說出下一張應選哪張卡。
禁止透露任何選擇題的正確選項或答案代號。
""",

    "case1_supply_completed": """
玩家已完成平台媒合流程與供給判斷活動。
玩家已理解：平台可以協助擁有既有閒置容量的一方，與有需求的一方建立連結。
玩家已理解：共享情境的重要特徵之一，是讓原本已存在但未充分利用的資源，被有需求的人使用。
可以討論媒合與閒置容量概念。
不要使用 A、B、C 或其他答案代號描述玩家剛才的作答。
不要提前透露尚未出現的永續總結內容。
""",

    "case1_summary_completed": """
玩家已完成 Case 1 的平台媒合與永續概念總結。
玩家已理解閒置容量、供需媒合，以及提高既有資源利用程度等概念。
玩家也已理解：共享模式不代表必然產生永續效果，仍需考慮實際使用方式與可能增加的額外需求。
可以協助玩家整理目前已經學過的 Case 1 概念。
不要提前透露 Case1Travel 尚未實際揭露的劇情內容。
"""
}
# ======================================
   
# ======================================
# AI 防暴雷：Case 2 各劇情進度允許使用的資訊
# ======================================
CASE2_REVEALED_INFO = {
    "case2_intro": """
玩家目前已進入 Case 2。
玩家已知道這學期需要《管理資訊系統》教材。
新書價格為 3500 元。
玩家只需要使用這本書一學期。
二手書價格為 500 元。
玩家目前只剩 500 元。
目前尚未完成「所有權 vs 使用權」教學。
""",

    "case2_ownership_usage_completed": """
玩家已知道這學期需要《管理資訊系統》教材。
新書價格為 3500 元，二手書價格為 500 元，玩家目前只剩 500 元。
玩家只需要使用這本書一學期。
玩家已完成「所有權 vs 使用權」教學。
玩家已理解：購買通常取得資產所有權；租借則是在特定期間取得使用權。
目前不要提前透露下一段會出現的人物或事件。
""",

    "case2_senior_revealed": """
玩家已完成「所有權 vs 使用權」教學。
玩家已遇見學長。
玩家已知道學長手上有一本自己已經不再使用、原本可能繼續放著的課本。
玩家可以開始思考：一項仍然可以使用、但目前沒有人使用的資產，是否仍然具有價值。
目前玩家尚未完成「閒置資產」教學，因此可以用問題引導，但不要直接替玩家完成後續教學結論。
""",

    "case2_idle_asset_completed": """
玩家已理解所有權與使用權的差異。
玩家已遇見學長，並知道學長有一本自己已經不再使用的課本。
玩家已完成「閒置資產」教學。
玩家已理解：資產目前沒有被原持有人使用，不代表它失去使用價值。
仍有使用價值、但目前未被充分使用的資產，可能形成閒置資產。
""",

    "case2_idle_reuse_completed": """
玩家已理解所有權與使用權。
玩家已理解閒置資產。
玩家已完成「閒置資產再利用」教學。
玩家已理解：既有閒置資源可以在不同時間重新被其他有需求的人使用，提高資產利用率。
""",

    "case2_sharing_economy_completed": """
玩家已完成 Case 2 的共享經濟辨識教學。
玩家已理解所有權與使用權、閒置資產、資產再利用。
玩家已理解：不能只因為一項服務使用「租借」形式，就直接判斷它是共享經濟。
判斷時還需要觀察資源由誰提供、資產原本是否已存在且處於未充分利用狀態，以及是否讓既有資產重新被利用。
""",

    "case2_platform_revealed": """
玩家已完成所有權與使用權、閒置資產、資產再利用，以及共享經濟辨識的相關教學。
玩家回到攤位後，已發現現場沒有自己需要的《管理資訊系統》課本。
玩家現在已得知「書人不輸陣」這個書籍共享平台。
可以引導玩家思考：平台如何把手上有閒置書籍的人，與暫時需要使用書籍的人連結起來。
不要提前透露後續天平活動的答案或 Case 2 最終結論。
""",

    "case2_scale_completed": """
玩家已完成 Case 2 的天平活動。
玩家已理解：自己的核心需求不是一定要擁有一本書，而是在這個學期取得可使用教材的機會。
玩家已理解租借可以提供一定期間的使用權，而不一定需要移轉所有權。
玩家已理解既有但未充分利用的資產，可以透過適當媒合重新提供給有需求的人使用。
可以協助玩家整理 Case 2 已經學過的概念，但仍不要直接代替玩家回答尚未作答的題目。
"""
    
    }
    # ======================================    

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

        # ======================================
        # AI 防暴雷：依 Case 1 劇情進度限制可見資訊
        # ======================================
        if "case1" in case_id_lower:
            case_context = CASE1_REVEALED_INFO.get(
                req.story_step,
                CASE1_REVEALED_INFO["case1_car_started"]
            )
        # ======================================

        # ======================================
        # AI 防暴雷：依目前劇情進度限制 Case 2 可見資訊
        # ======================================
        if "case2" in case_id_lower:
            case_context = CASE2_REVEALED_INFO.get(
                req.story_step,
                CASE2_REVEALED_INFO["case2_intro"]
            )

        print(f"[AI 防暴雷] case_id = {req.case_id}")
        print(f"[AI 防暴雷] story_step = {req.story_step}")
        # ======================================
        
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