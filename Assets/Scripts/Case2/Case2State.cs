public static class Case2State
{
    // 攤位階段：
    // 0 = 第三幕：初見攤位 (二手書 vs 原文書租借)
    // 1 = 第四幕：學長登場贈送經濟學課本
    // 2 = 第六幕：從教學 4 返回，準備前往天平
    public static int StallPhase = 0;

    // 教學場景啟動階段：
    // 0 = 第三幕進來：只玩教學 1 (所有權 vs 使用權)
    // 1 = 第五幕進來：從教學 2 開始輪播 (閒置資產 -> 資產再利用 -> 租賃與共享)
    public static int TeachStartPhase = 0;
}