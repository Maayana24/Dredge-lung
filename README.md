# Dredge lung test
Documentation for OOP finals
Maayan ben shitrit


Game concept -
In the past, politicians thought it was a good idea to bury nuclear waste deep in the mariana trench. Years after that strange singles are coming from deep inside.
You are a scientist, tasked with diving deep into the mariana trench in order to hunt the mutated fish that lives there in order to learn more about what's causing the anomalies and hopefully find a solution before it's too late. Just don't get hit by a rock before you reach the bottom.
How to run -
You need to install the dogica font I included in the folder to the computer fonts!!!!!
After that simply load the project :)


How to play -
Move with WASD, Aim harpoon with mouse and fire harpoon with left click.
Catch the mutated fish with the anomalies to score points.
Avoid the rocks, each rock hit will lose you a life.


Project structure -
I used a game manager to keep most of the game1 class as clean as possible and used singleton managers to organise separate logic. 
Interfaces:
IClickable: For all buttons and other clickable objects
 void Click() For click events
 bool IsMouseOver() For logic when mouse is over the object

IDrawable - For all drawable objects
void Draw() All objects that are drawable should have a Draw method
IUpdatable - For all Updatable objects
void Update() All objects that are drawable should have an Update method
ILayerable - For all Objects that are gets drawn and should keep track of layer depth
void UpdateLayerDepth() To update the depth of the object’s layer
ICollidable - For all objects with collision
void OnCollision(ICollidable other) For Adding logic when colliding with a specific object

Base classes:
BaseSpawner : IUpdatable - An abstract class that used as a base for the game’s spawners
public virtual void Update() To check and spawn when it’s time for the next spawn, and to update all the active entities of the spawner
public virtual void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) To change the speed and spawn time of the entities according to the current level
protected float GetRandomSpawnTime() Returns a random spawn time in the chosen range for the next spawn
protected abstract void SpawnRandomEntity() Spawned the entity, different logic in each spawner
protected virtual void UpdateActiveEntities() Has a list of entities to remove and adds inactive or out of bound entities, than removes them
protected bool IsOutOfBounds(T entity) Checks if the entity’s position is outside the playable area
protected abstract Vector2 GetEntityPosition(T entity) Returns the entity’s position
protected abstract bool IsEntityActive(T entity) Returns true if the entity is active
 protected abstract void DeactivateEntity(T entity) Deactivates the entity
Sprite : ILayerable, IUpdatable, IDrawable - An abstract class that used as a base all the sprites
public virtual void UpdateLayerDepth() Updates the layer depth based on the class priority
public abstract void Update() the update class for the sprites
public virtual void Draw() has a base draw logic

UIElement : ILayerable, IUpdatable, IDrawable -  An abstract class that used as a base for all UI elements
 public virtual void UpdateLayerDepth() Updates UI layer depth, Should be 0 to be up front
public abstract void Update()
public abstract void Draw()

Entities:
Fish: Sprite, ICollidable - Represent the fish in the game
public override void Update() Updates th movement, collision, and sprite direction of the fish
public override void UpdateLayerDepth() Updates the anomalies layer to be slightly above the fish layer
private void Movement() basic one directional speed for the fish
private void UpdateCollisionRect() Updating the collision to follow the fish position
public override void Draw() Draws the fish and it’s anomalies
public void Deactivate() Deactivate the fish and unregister it from the collision manager

Rock : Sprite, ICollidable - Representing the rocks in the game
public override void Update() Updates the position and collision
public void OnCollision(ICollidable other) Deactivate the rock when colliding with player or the harpoon
public void Deactivate() Deactivate the rock
public override void Draw() draws the rock if active

Anomaly : ILayerable - Represents the anomalies that gets drawn on the fish
public void UpdateLayerDepth() Updating the anomaly layer depth based on the Z-index priority 
public void Draw(Vector2 position, Vector2 scale, SpriteEffects spriteEffect, float layerDepth) Draws the anomaly

Player : Sprite, ICollidable - Representing the player submarine character
public override void Update() Updates player’s speed, bounds and position
private void Movement() Handles the player’s movement. Stops movement when firing the harpoon and slowly move towards the default Y position while no movement input is detected
public void OnCollision(ICollidable other) Remove a life if colliding with a rock
public override void Draw() Draws the player

Harpoon : Sprite, ICollidable - Representing the harpoon, the player's weapon
public override void Update() Updates position, cooldown timer and collision according to the harpoon state
private void UpdateAiming() Updates the aim direction according to mouse
private void Fire() register with collision manager and calls to UpdateTipPosition() to start moving the harpoon
private void UpdateTipPosition() Moves the tip of the harpoon forward
UpdateCollisionRect() Keeps the collision  on the harpoon tip
RegisterWithCollisionManager() Register th harpoon with the collision manager
UnregisterFromCollisionManager() Unregister th harpoon with the collision manager
public void OnCollision(ICollidable other) When colliding with fish make it the caught fish and retract, when colliding with rocks retract
private void ProcessCaughtFish() Check if the fish has anomalies and add a point if it does and deactivate the fish
public override void UpdateLayerDepth() Updates the layer depth
public override void Draw() Draws  the harpoon line according to the harpoon’s state

UI
Text : UIElement Representing all UI text
public void SetText(string text) Setting the text
public string GetText() Getting the text
public override void Draw() Draws the text
public override void Update() 
Empty but exist because inherits from UIElement to join UIElement lists in the UI manager

Button : UIElement, IClickable Representing a button in UI
public override void Update() If visible, change to darker color when mouse is hovering and calls for click if being clicked
public void SetText(string text, Color textColor, float scale = 1.0f) Set the button’s text if it has any
public void SetTextPosition(Vector2 position) To set specific position for the text
private void CenterText() Automatically centering the text
public bool IsMouseOver() Check if the mouse is over the button
public void Click() Invoking click event
public override void Draw() Draws the button and button text if visible
private void UpdateInitialUI() Initializing UI texts
public void Update() Main update class that contains most of the classes update methods


Managers
GameManager: The main manager of the game
private void ConnectEvents() Connects all the methods to their events
private void AddUpdatables() Adds all updatable classes to updatable list
private void AddDrawables()  Adds all drawable classes to drawable list
private void UpdateHarpoonCooldownUI() Updates the harpoon cooldown text
private void UpdateEntities<T>(List<T> entities) where T : class - to update spawnable entities
private void OnGameOver(object sender, EventArgs e) Handles game over event
public void Reset() Resets game for a new run
public void Draw() Contains most of the classes draw methods

UIManager : IUpdatable, IDrawable -  Manages the UI
public void SetUpUI() Initialize UI elements
private void OnReplayClicked() Replay button callback
public void UpdateScoreText(int score) Update text
public void UpdateHighScoreText(int highScore) Update text
public void UpdateLivesText(int lives) Update text
public void UpdateCooldownText(bool isInCooldown, int remainingTime) Update text
public void ShowGameOver(bool isGameOver) //Make the  game over UI visible
public Button AddButton(Vector2 position, Texture2D texture, Color color, Action onClick, float scale = 1.0f) Creates a button and adds to relevant lists
public Text AddText(Vector2 position, string text, Color color, float scale) To create UI text
public void Update() Updates UI elements and tracks mouse clicking
private void MouseClicking() //Reacts when the mouse is clicked and check for clickable objects
public void AddElement(UIElement element) Adds a UI element to relevant lists
public void RemoveElement(UIElement element) Remove a UI element from relevant lists
public void Draw() Draws visible UI elements

IM (InputManager): Static class to manage All input
public static void Update() Updates mouse and keyboard parameters
public static bool IsKeyPressed(Keys key) Returns true if the checked key is pressed

ScoreManager: Manage and save score and high score
public void AddPoints(int points) Adds point
public void RemoveLife() Removes a player’s life
public void Reset() Reset score and lives
private void LoadHighScore() Load the saved high score
private void SaveHighScore() Saves the current high score
protected virtual void OnGameOver() Trigger GameOver event
protected virtual void OnScoreChanged() Trigger ScoreChanged event
protected virtual void OnLivesChanged() Trigger LivesChanged event
protected virtual void OnHighScoreChanged() Trigge HighScoreChangedr event

DifficultyManager : IUpdatable  Manage the difficulty progression
public void Update() Change to next level when the timer ends
public void IncreaseDifficulty() Increase current level and invoke DifficultyChanged event
public void Reset() Reset to first level and difficulty

CollisionManager: Manage collision between game objects
public void Register(ICollidable collidable) Register class to collidable class
public void Unregister(ICollidable collidable) Unregister class to collidable class
public void CheckCollisions() Checking collision between all collidable objects

BackgroundManager : ILayerable, IUpdatable, IDrawable - Manage the background
public void InitializeBackground() Initialize all the background layers
public void UpdateLayerDepth() Assigning the layer depths to background layers and border mask
public void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) Updates the background speed to match the rock movement
public void SetSpeedMultiplier(float multiplier) setting the speed multiplier
public void Update() Adjusting speed and updates layers
private Texture2D LoadAndRotateTexture(string assetName) Rotating the original Background texture to the correct direction for the game
public void DrawBorderMasks() Drawing a border mask to hide the spawning fish until they reach the playable area
public void Draw() Draws all the background layers

AnomalyManager -  Manage the anomalies
private void LoadTextures() Loads the anomaly textures
public List<Anomaly> GenerateAnomaliesForFish(Fish fish) Generating between 0 to 2 anomalies for a fish
private Texture2D GetTextureForAnomalyType(AnomalyType type) Get the texture according to the type of the anomaly

Misc:
RockSpawner : BaseSpawner<Rock> - In charge of spawning the rocks
protected override void SpawnRandomEntity() Spawn a random rock at the bottom of the screen
protected override Vector2 GetEntityPosition(Rock rock) Gets the rock position
protected override bool IsEntityActive(Rock rock) Checks if the rock is active
protected override void DeactivateEntity(Rock rock) Deactivate the rock

FishSpawner : BaseSpawner<Fish> - In charge of spawning the fish
public override void OnDifficultyChanged(int level, float speedMultiplier, float spawnRateMultiplier) Change spawn rate and speed based on difficulty 
protected override void SpawnRandomEntity() Spawn a jellyfish or other type of fish in random
private void SpawnJelly() Spawn a jellyfish in a random position at the bottom of the screen
private void SpawnFish(int fishType, bool leftToRight) Spawn a fish based on type and spawn direction
private (string fishName, Rectangle sourceRect, Vector2 scale, float speed) GetFishAttributes(int fishType) Returns the correct attributes based on the fish type
protected override Vector2 GetEntityPosition(Fish fish) Get fish position
protected override bool IsEntityActive(Fish fish) Check if the fish is active
protected override void DeactivateEntity(Fish fish) Deactivate fish

PlaybleArea: A static class In charge of determining the playable area of the game, where objects are spawned and the player can move
public static void Initialize(int width, int height) initialize the playable area size and bounds

Layer: Represents a layer of the parallax background
public void Update(float movement) Updates the two copies of the layer to move smoothly
public void UpdateLayerDepth() Updates the depth and priority 
public void Draw() Drawing the layer

Globals: a static class to store parameters that would be accessed in the whole project
public static void Update(GameTime gameTime) Updates DeltaTime

DebugRenderer: a static class in charge of debugging collision and bounds
public static void Initialize(GraphicsDevice graphicsDevice) Creating a simple texture for the lines
public static void DrawRectangle(Rectangle rectangle, Color color, float layerDepth = 0f) Drawing the rectangle

Game1: No added methods. Initializing the game manager and a few global parameters

Program: No change







