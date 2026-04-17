# CODE REVIEW

作成日: 2026-04-17  
レビュー方法: マルチエージェント並列レビュー（4エージェント）

---

## 総合評価: 4/10

| カテゴリ | 評価 |
|---|---|
| パフォーマンス | 3/10 |
| バグ耐性 | 3/10 |
| 設計・アーキテクチャ | 3/10 |
| 命名・可読性 | 4/10 |
| 機能の完成度 | 5/10 |

---

## CRITICAL — 即修正必須

| # | ファイル | 行 | 内容 |
|---|---|---|---|
| C-1 | `Assets/kento/Scripts/Player/PlayerColorChan.cs` | 17 | `GetComponent<Material>()` は動作しない。Materialはコンポーネントではなく、正しくは `GetComponent<Renderer>().material` |
| C-2 | `Assets/kento/Scripts/Joint/PlayerJoinedManager.cs` | 92〜99 | `joinDevices.Clear()` で全デバイスを削除している。`joinDevices.Remove(device)` が正しい。加えて行92の条件 `if (context.control.device != device)` は常にfalseになるため実質無効 |
| C-3 | `Assets/Miwa/scripts/SuddenDeathMode.cs` | 38〜51 | Reflectionでprivateフィールドを直接書き換えている。フィールド名変更で無音バグが発生する |
| C-4 | `Assets/Scripts/BouncyWall.cs` | 23〜24 | `FixedUpdate()` 毎フレームで `FindGameObjectsWithTag("Ball")` を呼び出している。重大なパフォーマンス問題 |
| C-5 | `Assets/Scripts/Loop.cs` | 11 | `InvokeRepeating()` で無限にシーン再読み込みを繰り返している。停止手段がない |
| C-6 | `Assets/kento/Scripts/CursorController.cs` | 25〜26 | `GameObject.Find("Canvas")` の結果にnullチェックなし。Canvasが存在しない場合即クラッシュ |

---

## HIGH — リリース前に修正

| # | ファイル | 行 | 内容 |
|---|---|---|---|
| H-1 | `Assets/kento/Scripts/Bot/BotPlayerController1.cs` | 361 | `CollectPlayers()` で毎フレーム `FindGameObjectsWithTag("Player")` を呼び出し。O(n)のコスト |
| H-2 | `Assets/kento/Script/PlayerScript/BOT/BOTController.cs` | 200 | `Serch()` 内で毎フレーム `FindGameObjectsWithTag("Player")` を呼び出し |
| H-3 | `Assets/kento/Scripts/Player/brink/PlayerController1.cs` | 259 | `Invoke("EndTackle", ...)` 使用。廃止予定パターン、エラー耐性ゼロ |
| H-4 | `Assets/kento/Scripts/Bot/BotPlayerController1.cs` | 281 | 同上、`Invoke()` パターン |
| H-5 | `Assets/kento/Scripts/Bot/BotPlayerController1.cs` | 220 | `FixedUpdate` 内で `Time.deltaTime` を使用。`Time.fixedDeltaTime` が正しい |
| H-6 | `Assets/kento/Scripts/Player/brink/Reception1.cs` | 55 | UpdateとFixedUpdate両方で `rb.linearVelocity` を上書き。レースコンディション |
| H-7 | `Assets/kento/Scripts/Player/brink/Reception1.cs` | 79〜80 | `rb.useGravity = true` を無条件で復元。元々gravity=falseのオブジェクトが壊れる |
| H-8 | `Assets/Miwa/scripts/GameManager_M.cs` | 448 | `FindFirstObjectByType<PauseManager>()` をキャッシュせず毎回呼び出し |
| H-9 | `Assets/Miwa/scripts/GameManager_M.cs` | 374 | サバイバー候補をインデックス `{0,1,2,3}` でハードコード。4人固定前提 |
| H-10 | `Assets/Miwa/scripts/GameManager_M.cs` | 504 | `Time.timeScale = 0f` の復帰処理が保証されていない |
| H-11 | `Assets/Scripts/Meteor.cs` | 30 | `Destroy(collision.gameObject)` でステージオブジェクトを破壊。ゲーム進行不可になるリスク |
| H-12 | `Assets/Scripts/Meteor_ver2.cs` | 38, 96 | `AudioSource AS` にnullチェックなし。未設定時に NullReferenceException |
| H-13 | `Assets/Scripts/knockback.cs` | 32, 39 | `KnockBack()` 内でRigidbodyのnullチェックなし |
| H-14 | `Assets/kento/Scripts/Player/brink/PlayerController1.cs` | 305〜306 | `Reception1` 取得後のnullチェックなし |
| H-15 | `Assets/kento/Scripts/Bot/BotPlayerController1.cs` | 320〜321 | 同上、`Reception1` のnullチェックなし |
| H-16 | `Assets/kento/Script/ItemScript/PlayerItemEffect.cs` | 53 | `effectValue == 0` の場合に除算エラー。バリデーション必要 |
| H-17 | `Assets/Miwa/scripts/PlayerHealth.cs` | 27 | `OnFallOut()` 直後に `Destroy(gameObject)`。GameManagerの処理完了前に破棄される |
| H-18 | `Assets/Miwa/scripts/GameManager_M.cs` | 121 | `_currentMode` がnullの場合に `is SurvivalMode` キャストが例外になる可能性 |

---

## MEDIUM — コード品質

| # | ファイル | 行 | 内容 |
|---|---|---|---|
| M-1 | `Assets/Miwa/scripts/GameManager_M.cs` | 全体(580行) | God Class: モード切替・排除・UI・サウンド・ノックバック倍率を1クラスで担当 |
| M-2 | `Assets/kento/Scripts/Player/brink/PlayerController1.cs` | 全体(330行) | God Class: 移動・チャージ・攻撃・アニメ・エフェクトを1クラスで担当 |
| M-3 | `Assets/Scripts/Meteor.cs` vs `Assets/Scripts/Meteor_ver2.cs` | — | ほぼ同一コードの重複。音あり/なしの差だけで2ファイル |
| M-4 | `Assets/kento/Scripts/Player/brink/Reception1.cs` vs `Assets/kento/Script/PlayerScript/Reception.cs` | — | 同上、ほぼ同一コードが2ファイルに重複 |
| M-5 | `Assets/Scripts/ConveyorCartManager.cs` | 74〜79 | エントリが2件の時、`while(next == _activeIndex)` が理論上無限ループになる可能性 |
| M-6 | `Assets/Scripts/BoundBall.cs` | 42 | 衝突のたびに加速率が乗算され、上限なしで指数関数的加速 |
| M-7 | `Assets/Scripts/Cart_Player.cs` | 43 | `StopAllCoroutines()` で前コルーチンが意図せずキャンセルされる可能性 |
| M-8 | `Assets/Scripts/ObjectSpawner.cs` | 40 | `originalRadius = 0.860137f` ハードコード。prefabのスケール変更で無効化 |
| M-9 | `Assets/Scripts/ObjectSpawner.cs` | 97 | 距離チェックが `Vector3.zero`（ワールド原点）基準。スポナー位置と無関係 |
| M-10 | `Assets/Scripts/command.cs` | 55〜73 | キー検査ロジックが2回実行されている。冗長 |
| M-11 | `Assets/kento/Scripts/Player/UICameraFollower.cs` | 2 | `using NUnit.Framework;` が不要にインポートされている |
| M-12 | `Assets/Miwa/scripts/GameManager_M.cs` | 86 | 全角スペースがコード中に混入 |
| M-13 | `Assets/kento/Scripts/Joint/PlayerJoinedManager.cs` | 19〜24 | 複数行に全角スペースが混入 |
| M-14 | `Assets/kento/Scripts/Joint/PlayerDataHolder.cs` | 25 | パラメータ名が `cout`（C++の出力ストリーム名）。`count` のタイポ |
| M-15 | 全体 | — | マジックナンバー多数: `0.860137f`, `1.2f`, `0.01f`, `10.0f` 等。ScriptableObjectへの外出し推奨 |
| M-16 | 全体 | — | `GetComponent<>()` のnullチェックなしが多数。NullReferenceException多発リスク |
| M-17 | `Assets/kento/Scripts/Miwa/scripts/PanelFocusController.cs` | 16 | `Invoke(nameof(SetFocus), 0.01f)` のハードコード遅延。フレームベース待機の方が確実 |

---

## LOW — 命名・構造・整理

| # | 内容 |
|---|---|
| L-1 | タイポ（クラス名）: `Seencer` → Sensor、`EfectController` → EffectController、`AtackController` → AttackController、`OnAtatck` → OnAttack、`chage` → charge、`moveScrit` → moveScript |
| L-2 | タイポ（ファイル名）: `SpwanAndMoveDown.cs` → `SpawnAndMoveDown.cs` |
| L-3 | ファイル名とクラス名不一致: `BoundBall.cs` (中身は `AcceleratingBall`)、`DollyCartFollowPath.cs` (中身は `SplineCartFollowPath`)、`move.cs` (中身は `TestBall`)、`command.cs` (中身は `Command`) |
| L-4 | 小文字命名（C#規約違反）: `conveyor.cs`/`conveyor`、`knockback.cs`/`knockback` |
| L-5 | `Assets/kento/Script/` と `Assets/kento/Scripts/` が両方存在。命名不統一 |
| L-6 | `Assets/Scripts/おふざけ.cs` — 日本語ファイル名。ビルド環境によっては問題になる可能性 |
| L-7 | `PlayerController1` と `BotPlayerController1` で入力処理の仕組みが全く異なる（Actionコールバック vs Updateループ）。共通基底クラスなし |
| L-8 | `Item.cs` / `ItemController.cs` / `PlayerItemEffect.cs` が存在するがアイテム効果が未実装。系が中途半端 |
| L-9 | 空のUnityイベントメソッドが複数: `RenderColor.cs:Update()`、`PlayerColorChan.cs:Update()`、`CursorController.cs:Start()`、`MainGameManger.cs:Update()` |
| L-10 | `ChargeSpike.cs` 内にコメントアウトされた大量のデッドコード（22〜69行） |
| L-11 | `Item.cs` 内にコメントアウトされたコンストラクタ（24〜32行）。ScriptableObjectにはカスタムコンストラクタ不要 |
