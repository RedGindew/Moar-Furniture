using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ExtremelySimpleLogger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLEM.Data;
using MLEM.Data.Content;
using MLEM.Textures;
using MLEM.Ui;
using MLEM.Ui.Elements;
using TinyLife;
using TinyLife.Actions;
using TinyLife.Emotions;
using TinyLife.Mods;
using TinyLife.Objects;
using TinyLife.Utilities;
using TinyLife.World;
using TinyLife.Tools;
using Action = TinyLife.Actions.Action;
using MLEM.Misc;

namespace MoarFurniture;

public class MoarFurniture : Mod
{

    // the logger that we can use to log info about this mod
    public static Logger Logger { get; private set; }

    // visual data about this mod
    public override string Name => "Moar Furniture";
    public override string Description => "More of each furniture!";
    public override string IssueTrackerUrl => "https://x.com/RedGindew";
    public override string TestedVersionRange => "[0.46.0]";
    private Dictionary<Point, TextureRegion> uiTextures, openings, wallpaperTextures, tiles;
    public override TextureRegion Icon => this.uiTextures[new Point(0, 0)];


    public override void Initialize(Logger logger, RawContentManager content, RuntimeTexturePacker texturePacker, ModInfo info)
    {
        MoarFurniture.Logger = logger;
        // Used as windows Texture
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("Openings"), 8, 10), r => this.openings = r, 1, true);
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("UITex"), 8, 8), r => this.uiTextures = r, 1, true);
        texturePacker.Add(new UniformTextureAtlas(content.Load<Texture2D>("Tiles"), 8, 8), r => this.tiles = r, 1, true);
        texturePacker.Add(WallMode.ApplyMasks(content.Load<Texture2D>("Wallpapers"), 4, 10), r => this.wallpaperTextures = r, 1, true, true);
    }

    public override void AddGameContent(GameImpl game, ModInfo info)
    {
        // TABLE Set
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMDiningTable", new Point(1, 1), ObjectCategory.Table, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.DiningRoom),
            Colors = new ColorSettings(ColorScheme.SimpleWood) { Defaults = new int[] { 1 } },
            ObjectSpots = ObjectSpot.TableSpots(new Point(1, 1)).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMBedsideTable", new Point(1, 1), ObjectCategory.Table, 75)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bedroom),
            Colors = new ColorSettings(ColorScheme.SimpleWood) { Defaults = new int[] { 1 } },
            ObjectSpots = ObjectSpot.TableSpots(new Point(33, 5)).ToArray()
        });
        //FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMCoffeeTable", new Point(1, 1), ObjectCategory.Table, 60, ColorScheme.SimpleWood) {
        //Icon = this.Icon,
        //Colors = new int[]{1},
        //ObjectSpots = ObjectSpot.TableSpots(new Point(24, 13)).ToArray()
        //ObjectSpots = new ObjectSpot(new Vector2(12, 6), Predicate<FurnitureType> FurnitureType.Television, 0, new Direction2[]{Direction2.Right, Direction2.Left})
        //});
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMBedsideDrawers", new Point(1, 1), ObjectCategory.Table | ObjectCategory.Wardrobe, 75)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bedroom),
            Colors = new ColorSettings(ColorScheme.SimpleWood) { Defaults = new int[] { 1 } },
            ObjectSpots = ObjectSpot.TableSpots(new Point(33, 5)).ToArray(),
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMLargeDrawers", new Point(1, 1), ObjectCategory.Wardrobe, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bedroom),
            Colors = new ColorSettings(ColorScheme.SimpleWood){ Defaults = new int[] { 1 } },
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMBookcase", new Point(1, 1), ObjectCategory.Bookshelf, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Office),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.White){ Defaults = new int[] { 1, 0 } },
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.TALMSmallBookcase", new Point(1, 1), ObjectCategory.Bookshelf, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Office),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.White){ Defaults = new int[] { 1, 0 } },
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICChair", new Point(1, 1), ObjectCategory.Chair, 75)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.DiningRoom),
            Colors = new ColorSettings(ColorScheme.SimpleWood) { Map = new int[] { 0, 0 }, Defaults = new int[] { 1 }  },
            ObjectSpots = { },
            ActionSpots = new[] {new ActionSpot(Vector2.Zero, 0.1F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    }
                },
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        // COFFEE Set
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.CAFECoffeeDecor", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Colors = new ColorSettings(ColorScheme.Modern, ColorScheme.Modern){ Defaults = new int[] { 1, 2 }, PreviewName = "MoarFurniture.CAFECoffeeDecor" },
            Tab = (FurnitureTool.Tab.Decoration)
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.CAFECoffeeMachine", new Point(1, 1), ObjectCategory.CoffeeMachine | ObjectCategory.DisallowedOnGround | ObjectCategory.NonColliding | ObjectCategory.CounterObject, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White){ Defaults = new int[] { 4, 0 } },
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ElectricityRating = 2,
            WaterRating = 1
        });
        // KITCHEN Set
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SHARPFridge", new Point(1, 1), ObjectCategory.Fridge, 800)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            Colors = new ColorSettings(ColorScheme.Pastel){ Defaults = new int[] { 13 } },
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ElectricityRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SHARPOven", new Point(1, 1), ObjectCategory.Stove | ObjectCategory.Oven, 600)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.Pastel, ColorScheme.White){ Defaults = new int[] { 18, 18, 0 }, PreviewName = "MoarFurniture.SHARPOven" },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ElectricityRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICBuiltInOven", new Point(1, 1), ObjectCategory.Stove | ObjectCategory.DisallowedOnGround | ObjectCategory.Oven | ObjectCategory.CounterObject, 800)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White){ Defaults = new int[] { 18, 0 }, PreviewName = "MoarFurniture.BASICBuiltInOven" },
            ElectricityRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SHARPKitchenSink", new Point(1, 1), ObjectCategory.CounterObject | ObjectCategory.Sink, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            Colors = new ColorSettings(ColorScheme.Pastel){ Defaults = new int[] { 13 } },
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            WaterRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SHARPGlassCounters", new Point(1, 1), ObjectCategory.Counter, 80)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ConstructedType = typeof(CornerFurniture.Counter),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.SimpleWood, ColorScheme.White) { Defaults = new int[] { 1, 0, 0 }, PreviewName = "MoarFurniture.SHARPGlassCounters" },
            ObjectSpots = ObjectSpot.CounterSpots(false).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICCounter", new Point(1, 1), ObjectCategory.Counter, 60)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.SimpleWood){Defaults = new int[] { 1, 0 }, PreviewName = "MoarFurniture.BASICCounter" },
            ConstructedType = typeof(CornerFurniture.Counter),
            ObjectSpots = ObjectSpot.CounterSpots(false).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICFridge", new Point(1, 1), ObjectCategory.Fridge, 400)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            Colors = new ColorSettings(ColorScheme.Pastel){Defaults = new int[] { 13 }},
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ElectricityRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICApronSink", new Point(1, 1), ObjectCategory.CounterObject| ObjectCategory.DisallowedOnGround | ObjectCategory.Sink, 350)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            Colors = new ColorSettings(ColorScheme.Pastel){Defaults = new int[] { 13 }},
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            WaterRating = 2
        });
        // MOVIE / GAME Posters
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.MAWSMoviePoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.OJGamePoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.OJ2GamePoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.OJ3GamePoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.OJ4GamePoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.PlanetWarsPoster", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICFrame", new Point(1, 1), ObjectCategory.WallHanging, 120)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICTrioFrame", new Point(1, 1), ObjectCategory.WallHanging, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICPersonFrame", new Point(1, 1), ObjectCategory.WallHanging, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.CAFEHangingMenuBoard", new Point(1, 1), ObjectCategory.WallHanging, 160)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White, ColorScheme.SimpleWood){Defaults = new int[] { 0, 1 }, PreviewName = "MoarFurniture.CAFEHangingMenuBoard" },
            DefaultRotation = MLEM.Maths.Direction2.Right
        });

        // BASIC Set
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICMailbox", new Point(1, 1), ObjectCategory.Mailbox, 100)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Other),
            Colors = new ColorSettings(ColorScheme.Grays, ColorScheme.SimpleWood, ColorScheme.Modern){Defaults = new int[] { 1, 1, 12 }, PreviewName = "MoarFurniture.BASICMailbox"},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICShower", new Point(1, 1), ObjectCategory.Shower, 400)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bathroom),
            DefaultRotation = MLEM.Maths.Direction2.Down,
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White) { Map = new int[] { 0, 0, 0, 1 }, Defaults = new int[] {13, 0} },
            ObjectSpots = { },
            ActionSpots = new[] {new ActionSpot(Vector2.Zero, -0.2F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    }
                },
            ElectricityRating = 2,
            WaterRating = 2
        });

        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICBathroomSink", new Point(1, 1), ObjectCategory.WallHanging | ObjectCategory.Sink, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bathroom),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White){Defaults = new int[] { 13, 0 }},
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            WaterRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICToilet", new Point(1, 1), ObjectCategory.WallHanging | ObjectCategory.Toilet, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bathroom),
            Colors = new ColorSettings(ColorScheme.Pastel) { Map = new int[] { 0, 0 }, Defaults = new int[] { 13 } },
            ObjectSpots = { },
            ActionSpots = new[] {new ActionSpot(Vector2.Zero, 0F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    }
                },
            DefaultRotation = MLEM.Maths.Direction2.Right,
            WaterRating = 2
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICBathroomCounter", new Point(1, 1), ObjectCategory.Counter, 40)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bathroom),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.SimpleWood){Defaults = new int[] { 1, 2 }, PreviewName = "MoarFurniture.BASICBathroomCounter"},
            ConstructedType = typeof(CornerFurniture.Counter),
            ObjectSpots = ObjectSpot.CounterSpots(false).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICPatternedRug", new Point(2, 2), ObjectCategory.GroundItem | ObjectCategory.NonColliding, 500)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.Pastel){Defaults = new int[] { 13, 1 }, PreviewName = "MoarFurniture.BASICPatternedRug"},
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICSmallerRug", new Point(1, 2), ObjectCategory.GroundItem | ObjectCategory.NonColliding, 400)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.Pastel){Defaults = new int[] { 13, 1 }, PreviewName = "MoarFurniture.BASICSmallerRug"},
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICDoorMat", new Point(1, 1), ObjectCategory.GroundItem | ObjectCategory.NonColliding, 180)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.Pastel){Defaults = new int[] { 13, 1 }, PreviewName = "MoarFurniture.BASICDoorMat"},
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.COTTAGEMeshRug", new Point(2, 2), ObjectCategory.GroundItem | ObjectCategory.NonColliding, 500)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White){Defaults = new int[] { 1, 0 }},
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BASICLoveSeat", new Point(2, 1), ObjectCategory.Chair, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.LivingRoom),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White, ColorScheme.White) { Map = new int[] { 0, 0, 1 }, Defaults = new int[] { 13, 0, 0 } },
            ObjectSpots = { },
            ActionSpots = new[] {
                new ActionSpot(new Vector2(0.5f, 0.05f), 0.1F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    },
                },
            DefaultRotation = MLEM.Maths.Direction2.Right,
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.CornerCabinet", new Point(1, 1), ObjectCategory.Table, 75)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.SimpleWood){Defaults = new int[] { 0, 1 }, PreviewName = "MoarFurniture.CornerCabinet"},
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ObjectSpots = ObjectSpot.TableSpots(new Point(33, 3)).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SimpleVase", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Colors = new ColorSettings(ColorScheme.Pastel){Defaults = new int[] { 1 }},
            Tab = (FurnitureTool.Tab.Decoration),
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.LilStorageJars", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }},

        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.WorkingWardrobe", new Point(2, 1), ObjectCategory.Wardrobe, 350)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Bedroom),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.Pastel, ColorScheme.Pastel, ColorScheme.Pastel, ColorScheme.Grays, ColorScheme.White){Defaults = new int[] { 1, 2, 3, 1, 0, 0 }, PreviewName = "MoarFurniture.WorkingWardrobe"},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.ShelvingUnit", new Point(2, 1), ObjectCategory.Bookshelf, 500)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Office),
            Colors = new ColorSettings(ColorScheme.WarmDark, ColorScheme.SimpleWood, ColorScheme.White){Defaults = new int[] { 1, 1, 0 }, PreviewName = "MoarFurniture.ShelvingUnit"},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.RetroTable", new Point(1, 1), ObjectCategory.Table, 200)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.DiningRoom),
            Colors = new ColorSettings(ColorScheme.Grays, ColorScheme.Modern){Defaults = new int[] { 1, 13 }, PreviewName = "MoarFurniture.RetroTable"},
            ObjectSpots = ObjectSpot.TableSpots(new Point(1, 1)).ToArray()
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.RetroStool", new Point(1, 1), ObjectCategory.Chair, 160)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.DiningRoom),
            Colors = new ColorSettings(ColorScheme.Grays, ColorScheme.Modern){Defaults = new int[] { 1, 13 }, PreviewName = "MoarFurniture.RetroStool"},
            ObjectSpots = { },
            ActionSpots = new[] {new ActionSpot(Vector2.Zero, 0.1F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    }
                },
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.RetroSauceSet", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.DecorCabinet", new Point(1, 1), ObjectCategory.Nothing, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.White){Defaults = new int[] { 1, 0 }},
            DefaultRotation = MLEM.Maths.Direction2.Right
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.PodRack", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.GameCollection", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.White){Defaults = new int[] { 0 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.CerealClutter", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.MutedPastels, ColorScheme.MutedPastels, ColorScheme.MutedPastels){Defaults = new int[] { 14, 13, 12 }, PreviewName = "MoarFurniture.CerealClutter"}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.PlainPouf", new Point(1, 1), ObjectCategory.Chair, 160)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.LivingRoom),
            Colors = new ColorSettings(ColorScheme.Pastel){Defaults = new int[] { 1 }},
            ObjectSpots = { },
            ActionSpots = new[] {new ActionSpot(Vector2.Zero, 0.1F, MLEM.Maths.Direction2Helper.Adjacent) {
                    DrawLayer = f => 0
                    }
                },
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SimpleDesk", new Point(2, 1), ObjectCategory.Table, 150)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Office),
            Colors = new ColorSettings(ColorScheme.SimpleWood){Defaults = new int[] { 1 }},
            ObjectSpots = ObjectSpot.DeskSpots(-0.6f, true, 1).ToArray()
        });

        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SmallPottedPlant", new Point(1, 1), ObjectCategory.Nothing, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.Plants, ColorScheme.Pastel){Defaults = new int[] { 1, 1, 1 }, PreviewName = "MoarFurniture.SmallPottedPlant"},
            DefaultRotation = MLEM.Maths.Direction2.Up
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.ThinPillar", new Point(1, 1), ObjectCategory.Nothing, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.SimpleWood){Defaults = new int[] { 1 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.ThickPillar", new Point(1, 1), ObjectCategory.Nothing, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.SimpleWood){Defaults = new int[] { 1 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.ClassicalPillar", new Point(1, 1), ObjectCategory.Nothing, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Decoration),
            Colors = new ColorSettings(ColorScheme.Grays, ColorScheme.White){Defaults = new int[] { 1, 0 }}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.PalmTree", new Point(1, 1), ObjectCategory.Tree, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Outside),
            Colors = new ColorSettings(ColorScheme.SimpleWood, ColorScheme.Plants){Defaults = new int[] { 1, 1 }, PreviewName = "MoarFurniture.PalmTree"}
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BookStack", new Point(1, 1), ObjectCategory.DisallowedOnGround | ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Colors = new ColorSettings(ColorScheme.White, ColorScheme.Modern, ColorScheme.Modern, ColorScheme.Modern){Defaults = new int[] { 0, 1, 2, 3 }, PreviewName = "MoarFurniture.BookStack"},
            Tab = (FurnitureTool.Tab.Decoration),
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.SimpleRock", new Point(1, 1), ObjectCategory.SmallObject, 50)
        {
            Icon = this.Icon,
            Colors = new ColorSettings(ColorScheme.Grays){Defaults = new int[] { 0 }},
            Tab = (FurnitureTool.Tab.Outside),
        });
        FurnitureType.Register(new FurnitureType.TypeSettings("MoarFurniture.BudgetOven", new Point(1, 1), ObjectCategory.Stove | ObjectCategory.Oven, 250)
        {
            Icon = this.Icon,
            Tab = (FurnitureTool.Tab.Kitchen),
            ActionSpots = new[] { new ActionSpot(Vector2.Zero, MLEM.Maths.Direction2.Up) },
            Colors = new ColorSettings(ColorScheme.Pastel, ColorScheme.White){Defaults = new int[] { 13, 0 }},
            //Colors = new int[] { 13, 0 },
            ObjectSpots = ObjectSpot.CounterSpots(true),
            DefaultRotation = MLEM.Maths.Direction2.Right,
            ElectricityRating = 2
        });
        Wallpaper.Register("MoarFurniture.FABPlainWallPaper", 20, this.wallpaperTextures, new Point(0, 0), new ColorScheme[] { ColorScheme.Modern, ColorScheme.SimpleWood }, this.Icon, new int[] { 10, 8 });
        Wallpaper.Register("MoarFurniture.FABMidBorderedWallPaper", 20, this.wallpaperTextures, new Point(0, 1), new ColorScheme[] { ColorScheme.Modern, ColorScheme.SimpleWood }, this.Icon, new int[] { 10, 8 });
        Wallpaper.Register("MoarFurniture.FABRibbonWallPaper", 20, this.wallpaperTextures, new Point(0, 2), new ColorScheme[] { ColorScheme.Modern, ColorScheme.Modern, ColorScheme.SimpleWood }, this.Icon, new int[] { 10, 17, 8 });
        Wallpaper.Register("MoarFurniture.SeasonalTreeWallpaper", 20, this.wallpaperTextures, new Point(0, 3), new ColorScheme[] { ColorScheme.Modern, ColorScheme.Modern, ColorScheme.Modern, ColorScheme.Modern }, this.Icon, new int[] { 3, 15, 1, 9 });
        Wallpaper.Register("MoarFurniture.FABWallPanelling", 20, this.wallpaperTextures, new Point(0, 4), new ColorScheme[] { ColorScheme.SimpleWood }, this.Icon, new int[] { 1 });
    }

    public override IEnumerable<string> GetCustomFurnitureTextures(ModInfo info)
    {
        yield return "CAFECoffeeMachine";
        yield return "CAFECoffeeDecor";
        yield return "CAFEHangingMenuBoard";

        yield return "TALMBedsideDrawers";
        yield return "TALMBedsideTable";
        yield return "TALMCoffeeTable";
        yield return "TALMDiningTable";
        yield return "TALMLargeDrawers";
        yield return "TALMBookcase";
        yield return "TALMSmallBookcase";
        yield return "BASICPersonFrame";
        yield return "BASICLoveSeat";

        yield return "SHARPFridge";
        yield return "SHARPOven";
        yield return "SHARPKitchenSink";
        yield return "SHARPGlassCounters";

        yield return "MAWSMoviePoster";
        yield return "OJGamePoster";
        yield return "OJ2GamePoster";
        yield return "OJ3GamePoster";
        yield return "OJ4GamePoster";
        yield return "PlanetWarsPoster";
        yield return "BASICFrame";
        yield return "BASICTrioFrame";

        yield return "BASICMailbox";
        yield return "BASICCounter";
        yield return "BASICChair";
        yield return "BASICBuiltInOven";
        yield return "BASICFridge";
        yield return "BASICApronSink";
        yield return "BASICShower";
        yield return "BASICBathroomSink";
        yield return "BASICToilet";
        yield return "BASICBathroomCounter";

        yield return "BASICPatternedRug";
        yield return "BASICSmallerRug";
        yield return "BASICDoorMat";
        yield return "SimpleVase";
        yield return "LilStorageJars";

        yield return "COTTAGEMeshRug";
        yield return "CornerCabinet";
        yield return "ShelvingUnit";
        yield return "RetroTable";
        yield return "RetroStool";
        yield return "RetroSauceSet";
        yield return "DecorCabinet";
        yield return "GameCollection";
        yield return "PodRack";
        yield return "CerealClutter";
        yield return "PlainPouf";
        yield return "SimpleDesk";
        yield return "SmallPottedPlant";
        yield return "BookStack";
        yield return "ThinPillar";
        yield return "ThickPillar";
        yield return "ClassicalPillar";
        yield return "BudgetOven";
        yield return "WorkingWardrobe";
        yield return "PalmTree";
        yield return "SimpleRock";
    }
}