# コードレビュー

> 作成日: 2026-04-17 ／ レビュー方法: マルチエージェント並列レビュー（4エージェント）

---

## 総合評価

**4 / 10**

| カテゴリ | 評価 |
|---|---|
| パフォーマンス | 3/10 |
| バグ耐性 | 3/10 |
| 設計・アーキテクチャ | 3/10 |
| 命名・可読性 | 4/10 |
| 機能の完成度 | 5/10 |

プロトタイプとしてゲームループは機能しているが、新機能を追加する前にいくつかの根本的な問題を修正しておくことを強く推奨する。

---

## 🔴 CRITICAL — 即修正必須

放置するとクラッシュや機能停止に直結する問題。

---

### C-1｜Material の取得方法が間違っている
**`PlayerColorChan.cs` — 17行目**

`GetComponent<Material>()` は動作しない。`Material` はコンポーネントではないため、`GetComponent<Renderer>().material` で取得する必要がある。現状では色の変更が一切機能していない。

---

### C-2｜プレイヤー離脱時に全員まとめて削除される
**`PlayerJoinedManager.cs` — 92〜99行目**

`joinDevices.Clear()` で全デバイスをまとめて削除してしまっている。本来は `joinDevices.Remove(device)` で離脱した1人だけを削除すべき。さらに92行目の条件 `if (context.control.device != device)` が常に false になっているため、実質どのプレイヤーが抜けても全員のデバイスが消える。

---

### C-3｜Reflection でプライベートフィールドを書き換えている
**`SuddenDeathMode.cs` — 38〜51行目**

`PlayerController1` の `private` フィールドを Reflection（リフレクション）で強引に書き換えている。フィールド名を少し変えるだけでエラーが出ずに処理がスキップされるため、突然死モードが無音・無効になるバグが気づかないまま発生しうる。

---

### C-4｜BouncyWall が毎フレーム全オブジェクトを検索している
**`BouncyWall.cs` — 23〜24行目**

`FixedUpdate()` の中で `FindGameObjectsWithTag("Ball")` を毎フレーム呼び出している。これはシーン全体を毎フレームスキャンする重い処理で、ボールが増えるほどフレームレートが落ちる。

---

### C-5｜Loop.cs がシーンを無限リロードし続ける
**`Loop.cs` — 11行目**

`InvokeRepeating()` でシーン再読み込みを永遠に繰り返す処理が書かれており、停止する手段がない。意図的なデバッグコードなら削除必須。

---

### C-6｜Canvas が見つからないと即クラッシュ
**`CursorController.cs` — 25〜26行目**

`GameObject.Find("Canvas")` の戻り値を null チェックせずに直後で `.GetComponent<>()` を呼んでいる。Canvas が存在しないシーンで実行すると NullReferenceException でクラッシュする。

---

## 🟠 HIGH — リリース前に修正

安定性・パフォーマンスに関わる重要な問題。

---

### H-1・H-2｜BOT が毎フレームプレイヤーを全検索している
**`BotPlayerController1.cs` — 361行目 ／ `BOTController.cs` — 200行目**

どちらも Update / 検索メソッドの中で `FindGameObjectsWithTag("Player")` を毎フレーム呼んでいる。BOT が複数体いる場合は O(n²) になり、フレームレートへの影響が大きい。Start 時にキャッシュするか、イベント経由で管理する方式に変える必要がある。

---

### H-3・H-4｜Invoke() でタックル終了を制御している
**`PlayerController1.cs` — 259行目 ／ `BotPlayerController1.cs` — 281行目**

文字列でメソッド名を指定する `Invoke("EndTackle", ...)` を使っている。メソッド名を変えてもエラーが出ないため、タックルが永遠に終わらないバグが気づかずに発生しうる。コルーチンまたは UniTask での書き換えを推奨。

---

### H-5｜FixedUpdate 内で Time.deltaTime を使っている
**`BotPlayerController1.cs` — 220行目**

物理演算の FixedUpdate 内では `Time.fixedDeltaTime` を使う必要がある。`Time.deltaTime` を使うと、フレームレートが変動したときに BOT の回転速度がブレる。

---

### H-6｜Update と FixedUpdate で速度を同時に上書きしている
**`Reception1.cs` — 55行目**

Update で `rb.linearVelocity = Vector3.zero` を、FixedUpdate で `rb.linearVelocity = knockbackDir` を別々にセットしている。どちらが後に実行されるかは保証されないため、ノックバックがキャンセルされたり逆に止まらなくなったりするレースコンディションがある。

---

### H-7｜重力を強制的に true に戻している
**`Reception1.cs` — 79〜80行目**

スタン復帰時に `rb.useGravity = true` を無条件で設定している。もともと `useGravity = false` に設定されたオブジェクトに適用されると、意図しない落下が発生する。

---

### H-8〜H-10｜GameManager_M の複数の問題
**`GameManager_M.cs`**

- **448行目**: `FindFirstObjectByType<PauseManager>()` をキャッシュせず毎回呼び出している
- **374行目**: サバイバー候補のインデックスが `{0, 1, 2, 3}` でハードコードされており、4人固定前提の設計になっている
- **504行目**: `Time.timeScale = 0f` の復帰処理が保証されていない。何らかの理由でゲームが止まったまま再開できなくなるリスクがある

---

### H-11｜Meteor がステージオブジェクトを破壊する
**`Meteor.cs` — 30行目**

衝突時に `Destroy(collision.gameObject)` を呼んでいるため、隕石がステージの床や壁にぶつかったときにステージそのものが消えてゲームが続行不能になるリスクがある。

---

### H-12｜AudioSource の null チェックがない
**`Meteor_ver2.cs` — 38・96行目**

`AudioSource AS` が設定されていない場合、`AS.PlayOneShot()` で NullReferenceException が発生する。

---

### H-13〜H-15｜Rigidbody / Reception1 の null チェックがない
**`knockback.cs` — 32・39行目 ／ `PlayerController1.cs` — 305行目 ／ `BotPlayerController1.cs` — 320行目**

コンポーネント取得後のnullチェックが抜けている箇所が複数ある。対象オブジェクトの構成次第で NullReferenceException が発生する。

---

### H-16｜effectValue がゼロのとき除算エラーになる
**`PlayerItemEffect.cs` — 53行目**

`effectValue` が 0 の場合、除算でゼロ除算エラーが発生する。使用前に値のバリデーションが必要。

---

### H-17｜GameManager 処理前にオブジェクトが破棄される
**`PlayerHealth.cs` — 27行目**

`OnFallOut()` を呼んだ直後に `Destroy(gameObject)` を実行しているため、GameManager 側の処理が完了する前にオブジェクトが消えてしまう可能性がある。

---

### H-18｜_currentMode が null のときキャストが例外になる
**`GameManager_M.cs` — 121行目**

初期化タイミングによっては `_currentMode` が null のまま `is SurvivalMode` キャストが実行され、例外になる可能性がある。

---

## 🟡 MEDIUM — コード品質

直接クラッシュはしないが、保守や拡張の妨げになる問題。

---

### M-1・M-2｜God Class が2つある

`GameManager_M.cs`（580行）はモード切替・プレイヤー排除・UI・サウンド・ノックバック倍率をすべて1クラスで担っている。同様に `PlayerController1.cs`（330行）も移動・チャージ・攻撃・アニメ・エフェクトを1クラスで担っている。どちらも責務を分割しないと今後の機能追加や修正が困難になる。

---

### M-3・M-4｜ほぼ同じコードが2ファイルに重複している

- `Meteor.cs` と `Meteor_ver2.cs` — 音あり/なしの差だけで2ファイルに分かれている
- `Reception1.cs` と `Reception.cs` — 同上、ほぼ同じロジックが重複している

片方を修正したときにもう片方を直し忘れるバグの温床になる。

---

### M-5｜ConveyorCartManager で無限ループになりうる
**`ConveyorCartManager.cs` — 74〜79行目**

カートのエントリが2件しかない場合、`while (next == _activeIndex)` が理論上無限ループになる可能性がある。

---

### M-6｜BoundBall の加速に上限がない
**`BoundBall.cs` — 42行目**

衝突のたびに加速率が乗算される仕組みになっており、上限がないため連続衝突で速度が指数関数的に増大する。

---

### M-7｜StopAllCoroutines で別のコルーチンも止まる
**`Cart_Player.cs` — 43行目**

`StopAllCoroutines()` を呼んでいるため、意図していない他のコルーチンまで停止させてしまう可能性がある。

---

### M-8・M-9｜ObjectSpawner の設計の問題
**`ObjectSpawner.cs`**

- **40行目**: `originalRadius = 0.860137f` がハードコードされており、Prefab のスケールを変更すると値が合わなくなる
- **97行目**: 距離チェックの基準がワールド原点 `Vector3.zero` になっており、スポナー自体の位置と無関係

---

### M-10｜command.cs のキー検査が2重になっている
**`command.cs` — 55〜73行目**

同じキーのチェックロジックが2回実行されている。統合できる。

---

### M-11｜UICameraFollower にテスト用ライブラリが残っている
**`UICameraFollower.cs` — 2行目**

`using NUnit.Framework;` がインポートされているが、このスクリプトでは一切使用されていない。

---

### M-12・M-13｜コード内に全角スペースが混入している
**`GameManager_M.cs` — 86行目 ／ `PlayerJoinedManager.cs` — 19〜24行目**

全角スペースがコード中に混入している。エディタによっては構文エラーや予期しない動作の原因になる。

---

### M-14｜パラメータ名のタイポ
**`PlayerDataHolder.cs` — 25行目**

パラメータ名が `cout`（C++ の出力ストリームと同名）になっている。`count` のタイポと思われる。

---

### M-15｜マジックナンバーが多数散在している

`0.860137f`, `1.2f`, `0.01f`, `10.0f` など、意味の分からない数値がコード内に直書きされている箇所が多い。ScriptableObject や定数に外出しすることで意図が明確になる。

---

### M-16｜GetComponent の null チェックが全体的に不足している

`GetComponent<>()` の戻り値を null チェックせずに使っている箇所が多数ある。コンポーネントがアタッチされていない Prefab に適用したとき NullReferenceException が多発するリスクがある。

---

### M-17｜PanelFocusController のハードコード遅延
**`PanelFocusController.cs` — 16行目**

`Invoke(nameof(SetFocus), 0.01f)` の 0.01 秒遅延がハードコードされている。フレームベースの待機（`yield return null`）の方が環境に依存せず確実。

---

## 🔵 LOW — 命名・構造・整理

動作への影響は小さいが、コードの読みやすさや保守性に関わる問題。

---

### L-1｜クラス名のタイポ（複数）

| 現状 | 正しい表記 |
|---|---|
| `Seencer` | `Sensor` |
| `EfectController` | `EffectController` |
| `AtackController` | `AttackController` |
| `OnAtatck` | `OnAttack` |
| `chage` | `charge` |
| `moveScrit` | `moveScript` |

---

### L-2｜ファイル名のタイポ

`SpwanAndMoveDown.cs` → `SpawnAndMoveDown.cs`

---

### L-3｜ファイル名とクラス名が一致していない（複数）

| ファイル名 | 実際のクラス名 |
|---|---|
| `BoundBall.cs` | `AcceleratingBall` |
| `DollyCartFollowPath.cs` | `SplineCartFollowPath` |
| `move.cs` | `TestBall` |
| `command.cs` | `Command` |

Unity の規約ではファイル名とクラス名を一致させる必要がある。

---

### L-4｜小文字のクラス名（C# 規約違反）

`conveyor.cs` / `knockback.cs` はファイル名・クラス名ともに小文字始まり。C# の規約では PascalCase にすべき。

---

### L-5｜フォルダ名が不統一

`Assets/kento/Script/` と `Assets/kento/Scripts/` が両方存在している。どちらかに統一する。

---

### L-6｜日本語ファイル名

`Assets/Scripts/おふざけ.cs` — 一部のビルド環境やツールで問題になる可能性があるため、英語名に変更推奨。

---

### L-7｜Player と BOT で入力処理の仕組みが全く異なる

`PlayerController1` は InputAction コールバック方式、`BotPlayerController1` は Update ループ方式で設計が統一されていない。共通の基底クラスを作ることで管理しやすくなる。

---

### L-8｜アイテムシステムが中途半端

`Item.cs` / `ItemController.cs` / `PlayerItemEffect.cs` が存在するが、実際のアイテム効果が未実装のまま放置されている。

---

### L-9｜空の Unity イベントメソッドが複数残っている

何もしない `Update()` や `Start()` が以下のファイルに残っている。不要なら削除すべき。

- `RenderColor.cs`
- `PlayerColorChan.cs`
- `CursorController.cs`
- `MainGameManger.cs`

---

### L-10・L-11｜コメントアウトされたデッドコード

- `ChargeSpike.cs` — 22〜69行目にコメントアウトされた大量のコードが残っている
- `Item.cs` — 24〜32行目にコメントアウトされたコンストラクタが残っている（ScriptableObject にはカスタムコンストラクタは不要）
