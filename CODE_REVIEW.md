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

`GetComponent<Material>()` は動作しない。`Material` はコンポーネントではないため、現状では色の変更が一切機能していない。

> **修正案**
> `GetComponent<Renderer>().material` で取得する。インスタンスを返すため他のオブジェクトへの影響もない。

---

### C-2｜プレイヤー離脱時に全員まとめて削除される
**`PlayerJoinedManager.cs` — 92〜99行目**

`joinDevices.Clear()` で全デバイスをまとめて削除してしまっている。さらに92行目の条件 `if (context.control.device != device)` が常に false になっているため、実質どのプレイヤーが抜けても全員のデバイスが消える。

> **修正案**
> `joinDevices.Clear()` を `joinDevices.Remove(device)` に変更する。92行目の条件も改めて意図通りかを確認すること。

---

### C-3｜Reflection でプライベートフィールドを書き換えている
**`SuddenDeathMode.cs` — 38〜51行目**

`PlayerController1` の `private` フィールドを Reflection で強引に書き換えている。フィールド名を少し変えるだけでエラーが出ずに処理がスキップされるため、突然死モードが無音・無効になるバグが気づかないまま発生しうる。

> **修正案**
> `PlayerController1` 側に `public void SetKnockbackMultiplier(float value)` のような公開メソッドを用意して、そちら経由で値を変更する。

---

### C-4｜BouncyWall が毎フレーム全オブジェクトを検索している
**`BouncyWall.cs` — 23〜24行目**

`FixedUpdate()` の中で `FindGameObjectsWithTag("Ball")` を毎フレーム呼び出している。シーン全体を毎フレームスキャンする重い処理で、ボールが増えるほどフレームレートが落ちる。

> **修正案**
> ボールが生成・破棄されるタイミングで静的リストに登録・削除する管理クラスを作り、BouncyWall はそのリストを参照する。

---

### C-5｜Loop.cs がシーンを無限リロードし続ける
**`Loop.cs` — 11行目**

`InvokeRepeating()` でシーン再読み込みを永遠に繰り返す処理が書かれており、停止する手段がない。

> **修正案**
> デバッグ用のコードであれば削除する。実際にリロードが必要な場合は条件とキャンセル手段を明示的に実装する。

---

### C-6｜Canvas が見つからないと即クラッシュ
**`CursorController.cs` — 25〜26行目**

`GameObject.Find("Canvas")` の戻り値を null チェックせずに直後で `.GetComponent<>()` を呼んでいる。Canvas が存在しないシーンで実行すると NullReferenceException でクラッシュする。

> **修正案**
> `Find` の直後に `if (obj == null) return;` を追加する。または Inspector から直接アサインする `[SerializeField]` 方式に変更するとより安全。

---

## 🟠 HIGH — リリース前に修正

安定性・パフォーマンスに関わる重要な問題。

---

### H-1・H-2｜BOT が毎フレームプレイヤーを全検索している
**`BotPlayerController1.cs` — 361行目 ／ `BOTController.cs` — 200行目**

どちらも Update / 検索メソッドの中で `FindGameObjectsWithTag("Player")` を毎フレーム呼んでいる。BOT が複数体いる場合は O(n²) になり、フレームレートへの影響が大きい。

> **修正案**
> プレイヤーリストを管理する静的クラスまたは GameManager 経由で取得できるようにし、Start 時に一度だけキャッシュする。

---

### H-3・H-4｜Invoke() でタックル終了を制御している
**`PlayerController1.cs` — 259行目 ／ `BotPlayerController1.cs` — 281行目**

文字列でメソッド名を指定する `Invoke("EndTackle", ...)` を使っている。メソッド名を変えてもエラーが出ないため、タックルが永遠に終わらないバグが気づかずに発生しうる。

> **修正案**
> `StartCoroutine` + `WaitForSeconds` に置き換える。`Invoke` は原則使わない。

---

### H-5｜FixedUpdate 内で Time.deltaTime を使っている
**`BotPlayerController1.cs` — 220行目**

物理演算の FixedUpdate 内では `Time.fixedDeltaTime` を使う必要がある。`Time.deltaTime` を使うと、フレームレートが変動したときに BOT の回転速度がブレる。

> **修正案**
> `Time.deltaTime` を `Time.fixedDeltaTime` に変更する。

---

### H-6｜Update と FixedUpdate で速度を同時に上書きしている
**`Reception1.cs` — 55行目**

Update で `rb.linearVelocity = Vector3.zero` を、FixedUpdate で `rb.linearVelocity = knockbackDir` を別々にセットしており、どちらが後に実行されるか保証されないレースコンディションがある。

> **修正案**
> 速度の操作は FixedUpdate に一本化する。Update 側ではフラグを立てるだけにして、FixedUpdate 内でそのフラグを見て処理する。

---

### H-7｜重力を強制的に true に戻している
**`Reception1.cs` — 79〜80行目**

スタン復帰時に `rb.useGravity = true` を無条件で設定しており、もともと `useGravity = false` のオブジェクトに適用されると意図しない落下が発生する。

> **修正案**
> スタン開始時に元の `useGravity` の値を変数に退避しておき、復帰時にその値を戻す。

---

### H-8〜H-10｜GameManager_M の複数の問題
**`GameManager_M.cs`**

- **448行目**: `FindFirstObjectByType<PauseManager>()` をキャッシュせず毎回呼び出している
- **374行目**: サバイバー候補のインデックスが `{0, 1, 2, 3}` でハードコードされており、4人固定前提の設計になっている
- **504行目**: `Time.timeScale = 0f` の復帰処理が保証されていない

> **修正案**
> - 448: Start や Awake で `_pauseManager = FindFirstObjectByType<PauseManager>()` としてキャッシュする
> - 374: `activePlayers` リストから動的に生成する
> - 504: `finally` ブロックや明示的な復帰処理で `timeScale` の戻し漏れを防ぐ

---

### H-11｜Meteor がステージオブジェクトを破壊する
**`Meteor.cs` — 30行目**

衝突時に `Destroy(collision.gameObject)` を呼んでいるため、ステージの床や壁に当たったときにステージが消えてゲームが続行不能になるリスクがある。

> **修正案**
> 衝突相手のタグを確認し、破壊してよいオブジェクトのみ `Destroy` する。例: `if (collision.gameObject.CompareTag("Destructible"))` のように限定する。

---

### H-12｜AudioSource の null チェックがない
**`Meteor_ver2.cs` — 38・96行目**

`AudioSource AS` が設定されていない場合、`AS.PlayOneShot()` で NullReferenceException が発生する。

> **修正案**
> `if (AS != null) AS.PlayOneShot(...)` でガードする。または `[RequireComponent(typeof(AudioSource))]` を付けて未設定を防ぐ。

---

### H-13〜H-15｜Rigidbody / Reception1 の null チェックがない
**`knockback.cs` — 32・39行目 ／ `PlayerController1.cs` — 305行目 ／ `BotPlayerController1.cs` — 320行目**

コンポーネント取得後の null チェックが抜けている箇所が複数ある。

> **修正案**
> `GetComponent<>()` の直後に null チェックを追加する。頻繁に参照するなら Start 時にキャッシュして、null なら警告ログを出す。

---

### H-16｜effectValue がゼロのとき除算エラーになる
**`PlayerItemEffect.cs` — 53行目**

`effectValue` が 0 の場合、ゼロ除算エラーが発生する。

> **修正案**
> `if (effectValue == 0) return;` または `Mathf.Max(effectValue, 1)` でゼロを弾く。

---

### H-17｜GameManager 処理前にオブジェクトが破棄される
**`PlayerHealth.cs` — 27行目**

`OnFallOut()` を呼んだ直後に `Destroy(gameObject)` を実行しているため、GameManager 側の処理が完了する前にオブジェクトが消えてしまう可能性がある。

> **修正案**
> 即時 Destroy ではなく、GameManager 側の排除処理が完了したあとに Destroy を呼ぶ。コールバックや `Destroy(gameObject, 0.1f)` で遅延させるのも手。

---

### H-18｜_currentMode が null のときキャストが例外になる
**`GameManager_M.cs` — 121行目**

初期化タイミングによっては `_currentMode` が null のまま `is SurvivalMode` キャストが実行され、例外になる可能性がある。

> **修正案**
> `_currentMode != null && _currentMode is SurvivalMode` のように null チェックを先に入れる。

---

## 🟡 MEDIUM — コード品質

直接クラッシュはしないが、保守や拡張の妨げになる問題。

---

### M-1・M-2｜God Class が2つある

`GameManager_M.cs`（580行）はモード切替・プレイヤー排除・UI・サウンド・ノックバック倍率をすべて1クラスで担っている。同様に `PlayerController1.cs`（330行）も移動・チャージ・攻撃・アニメ・エフェクトを1クラスで担っている。どちらも責務を分割しないと今後の機能追加や修正が困難になる。

> **修正案**
> GameManager_M は `RoundManager`・`PlayerManager`・`UIManager` 等に分割する。PlayerController1 は移動・攻撃・アニメーションをそれぞれ別コンポーネントに切り出す。一度にやらず、機能を追加するタイミングで少しずつ分割していくのが現実的。

---

### M-3・M-4｜ほぼ同じコードが2ファイルに重複している

- `Meteor.cs` と `Meteor_ver2.cs` — 音あり/なしの差だけで2ファイルに分かれている
- `Reception1.cs` と `Reception.cs` — ほぼ同じロジックが重複している

> **修正案**
> 共通ロジックを1つのクラスにまとめ、音の有無は `[SerializeField]` の AudioSource が null かどうかで判定する。Reception 系も同様に1クラスに統合する。

---

### M-5｜ConveyorCartManager で無限ループになりうる
**`ConveyorCartManager.cs` — 74〜79行目**

カートのエントリが2件しかない場合、`while (next == _activeIndex)` が理論上無限ループになる可能性がある。

> **修正案**
> `while` ループに反復回数の上限を設けるか、エントリ数が1以下のときは処理をスキップする条件を加える。

---

### M-6｜BoundBall の加速に上限がない
**`BoundBall.cs` — 42行目**

衝突のたびに加速率が乗算される仕組みになっており、連続衝突で速度が指数関数的に増大する。

> **修正案**
> `Mathf.Clamp` で速度に上限を設ける。例: `rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed)`

---

### M-7｜StopAllCoroutines で別のコルーチンも止まる
**`Cart_Player.cs` — 43行目**

`StopAllCoroutines()` を呼んでいるため、意図していない他のコルーチンまで停止させてしまう可能性がある。

> **修正案**
> コルーチンを変数に保持し、`StopCoroutine(_hitstopCoroutine)` で対象を絞って停止する。

---

### M-8・M-9｜ObjectSpawner の設計の問題
**`ObjectSpawner.cs`**

- **40行目**: `originalRadius = 0.860137f` がハードコードされており、Prefab のスケールを変更すると値が合わなくなる
- **97行目**: 距離チェックの基準がワールド原点 `Vector3.zero` になっており、スポナー自体の位置と無関係

> **修正案**
> - 40行目: Start 時に Prefab の Collider から `bounds.extents` を使って動的に取得する
> - 97行目: `Vector3.zero` を `transform.position` に変更する

---

### M-10｜command.cs のキー検査が2重になっている
**`command.cs` — 55〜73行目**

同じキーのチェックロジックが2回実行されている。

> **修正案**
> 2つのループを1つにまとめ、正しいキーが来たときとそうでないときの処理を1ループ内で分岐させる。

---

### M-11｜UICameraFollower にテスト用ライブラリが残っている
**`UICameraFollower.cs` — 2行目**

`using NUnit.Framework;` がインポートされているが、このスクリプトでは一切使用されていない。

> **修正案**
> 該当の `using` 行を削除する。

---

### M-12・M-13｜コード内に全角スペースが混入している
**`GameManager_M.cs` — 86行目 ／ `PlayerJoinedManager.cs` — 19〜24行目**

全角スペースがコード中に混入している。エディタによっては構文エラーや予期しない動作の原因になる。

> **修正案**
> エディタの「全角スペースを表示する」設定をオンにして該当箇所を探し、半角スペースまたは削除に修正する。

---

### M-14｜パラメータ名のタイポ
**`PlayerDataHolder.cs` — 25行目**

パラメータ名が `cout`（C++ の出力ストリームと同名）になっている。`count` のタイポ。

> **修正案**
> `cout` → `count` に修正する。

---

### M-15｜マジックナンバーが多数散在している

`0.860137f`, `1.2f`, `0.01f`, `10.0f` など、意味の分からない数値がコード内に直書きされている箇所が多い。

> **修正案**
> ScriptableObject や `const` / `[SerializeField]` の変数に外出しして、意図が名前から分かるようにする。

---

### M-16｜GetComponent の null チェックが全体的に不足している

`GetComponent<>()` の戻り値を null チェックせずに使っている箇所が多数ある。

> **修正案**
> `var x = GetComponent<X>(); if (x == null) { Debug.LogWarning("..."); return; }` のパターンを徹底する。

---

### M-17｜PanelFocusController のハードコード遅延
**`PanelFocusController.cs` — 16行目**

`Invoke(nameof(SetFocus), 0.01f)` の 0.01 秒遅延がハードコードされている。

> **修正案**
> `yield return null` で1フレーム待つコルーチンに変更する。

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

> **修正案**
> 一括リネームする。Unity の場合、クラス名を変えると Prefab のコンポーネント参照が外れることがあるため、リネーム後に Prefab を確認する。

---

### L-2｜ファイル名のタイポ

`SpwanAndMoveDown.cs` → `SpawnAndMoveDown.cs`

> **修正案**
> ファイルをリネームし、Prefab のコンポーネント参照が外れていないか確認する。

---

### L-3｜ファイル名とクラス名が一致していない（複数）

| ファイル名 | 実際のクラス名 |
|---|---|
| `BoundBall.cs` | `AcceleratingBall` |
| `DollyCartFollowPath.cs` | `SplineCartFollowPath` |
| `move.cs` | `TestBall` |
| `command.cs` | `Command` |

> **修正案**
> ファイル名をクラス名に合わせてリネームする（または逆）。Unity の規約ではファイル名とクラス名を一致させる必要がある。

---

### L-4｜小文字のクラス名（C# 規約違反）

`conveyor.cs` / `knockback.cs` はファイル名・クラス名ともに小文字始まり。

> **修正案**
> ファイル名・クラス名ともに PascalCase（`Conveyor`、`Knockback`）に統一する。

---

### L-5｜フォルダ名が不統一

`Assets/kento/Script/` と `Assets/kento/Scripts/` が両方存在している。

> **修正案**
> どちらかに統一する。Unity でフォルダを移動する場合は必ず Unity Editor 上で行い、`.meta` ファイルが崩れないようにする。

---

### L-6｜日本語ファイル名

`Assets/Scripts/おふざけ.cs` — 一部のビルド環境やツールで問題になる可能性がある。

> **修正案**
> 英語名に変更する。内容に応じて `DebugMisc.cs` や `Sandbox.cs` など。

---

### L-7｜Player と BOT で入力処理の仕組みが全く異なる

`PlayerController1` は InputAction コールバック方式、`BotPlayerController1` は Update ループ方式で設計が統一されていない。

> **修正案**
> 共通の抽象クラス `BasePlayerController` を作り、移動・攻撃などの共通インターフェースを定義する。Player と BOT はそれを継承して入力ソースだけ差し替える形にする。

---

### L-8｜アイテムシステムが中途半端

`Item.cs` / `ItemController.cs` / `PlayerItemEffect.cs` が存在するが、実際のアイテム効果が未実装のまま放置されている。

> **修正案**
> 実装しないなら一旦ファイルごと削除してスッキリさせる。実装する場合は効果の基底クラス（`ItemEffect`）を作り、効果ごとにサブクラスを切る設計が拡張しやすい。

---

### L-9｜空の Unity イベントメソッドが複数残っている

何もしない `Update()` や `Start()` が以下のファイルに残っている。

- `RenderColor.cs`
- `PlayerColorChan.cs`
- `CursorController.cs`
- `MainGameManger.cs`

> **修正案**
> 削除する。Unity は空の `Update()` でも毎フレーム呼び出しのオーバーヘッドが発生するため、不要なら消した方がよい。

---

### L-10・L-11｜コメントアウトされたデッドコード

- `ChargeSpike.cs` — 22〜69行目にコメントアウトされた大量のコードが残っている
- `Item.cs` — 24〜32行目にコメントアウトされたコンストラクタが残っている（ScriptableObject にはカスタムコンストラクタは不要）

> **修正案**
> Git で管理しているため、過去のコードはいつでも履歴から取り出せる。コメントアウトのまま残す必要はないので削除する。
