using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tetris_Three
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //game logic 
        static int xGameSize = 13, yGameSize = 20;
        bool isWeCanMoveObject = true;
        List<Vector2> fallenSquares = new List<Vector2>();

        //game data
        long score = 0;

        //list of array of all Possible figures of this game
        List<Vector2[]> figures = new List<Vector2[]>();
        Vector2 figureStartPos = new Vector2(0, 0);
        /*
        0 - line horizontal
        1 - line vertical

        2 - horse top headed (1)
        3 - horse top headed (2)
        4 - horse top headed (3)
        5 - horse top headed (4)

        6 - horse bottom headed (1)
        7 - horse bottom headed (2)
        8 - horse bottom headed (3)
        9 - horse bottom headed (4)

        10 - pyramid (1)
        11 - pyramid (2)
        12 - pyramid (3)
        13 - pyramid (4)

        14 - Z (left) (1)
        15 - Z (left) (2)

        16 - Z (right) (1)
        17 - Z (right) (2)

        18 - tetr
        */

        //current falling figure data
        Vector2[] currentFallingFigurePosition = new Vector2[4];
        int currentFigureFalling = 0;

        //vectors of move action
        Vector2 DirTop = new Vector2(0, -1);
        Vector2 DirDown = new Vector2(0, 1);
        Vector2 DirDownFast = new Vector2(0, 6);
        Vector2 DirLeft = new Vector2(-3, 0);
        Vector2 DirRight = new Vector2(3, 0);

        //texture
        Texture2D squareTexture;

        //font
        SpriteFont font;

        //init game size
        private int width;
        private int height;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //sound
            Song song = Content.Load<Song>("sound/bg_sound");  // Put the name of your song here instead of "song_title"
            MediaPlayer.Play(song);

            //font 
            font = Content.Load<SpriteFont>("graphics/SpriteFont1");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //load texture
            squareTexture = Content.Load<Texture2D>("graphics/ic_simple_square");
            //set the game size
            width = squareTexture.Width * xGameSize;
            height = squareTexture.Height * yGameSize;
            graphics.PreferredBackBufferHeight = height;
            graphics.PreferredBackBufferWidth = width;
            graphics.ApplyChanges();

            //set the start point of each figures
            int middlePosition = 0;
            for (int i = 0; i < xGameSize / 2 - 1; i++) {
                middlePosition += squareTexture.Width;
            }

            figureStartPos = new Vector2(middlePosition, 0);

            //set default value of move square
            DirLeft = new Vector2(-squareTexture.Width, 0);
            DirRight = new Vector2(squareTexture.Width, 0);
            
            //init figures
            //line
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width*1, 0), new Vector2(squareTexture.Width*2, 0), new Vector2(squareTexture.Width*3, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height*1), new Vector2(0, squareTexture.Height*2), new Vector2(0, squareTexture.Height*3) });
            //horse (top head)
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 2, 0), new Vector2(squareTexture.Width * 2, squareTexture.Height * 1) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * -2), new Vector2(squareTexture.Width * 1, squareTexture.Height * -1), new Vector2(squareTexture.Width * 1, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1), new Vector2(squareTexture.Width * 2, squareTexture.Height * 1) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(0, squareTexture.Height * 2), new Vector2(squareTexture.Width * 1, 0) });
            //horse (bottom head)
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 2, squareTexture.Height * -1), new Vector2(squareTexture.Width * 2, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(0, squareTexture.Height * 2), new Vector2(squareTexture.Width * 1, squareTexture.Height * 2) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 2, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, squareTexture.Height * 2) });
            //pyramid
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1), new Vector2(squareTexture.Width * 2, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * -1), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * -1), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 2, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(0, squareTexture.Height * 2), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1) });
            // Z (left)
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, squareTexture.Height * 2) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * -1), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 2, squareTexture.Height * -1) });
            // Z (right)
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, squareTexture.Height * -1), new Vector2(squareTexture.Width * 1, 0) });
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1), new Vector2(squareTexture.Width * 2, squareTexture.Height * 1) });
            //tetr
            figures.Add(new Vector2[4] { new Vector2(0, 0), new Vector2(0, squareTexture.Height * 1), new Vector2(squareTexture.Width * 1, 0), new Vector2(squareTexture.Width * 1, squareTexture.Height * 1) });

            //drop first figure
            currentFigureFalling = (new Random(DateTime.Now.Millisecond)).Next(figures.Count);
            currentFallingFigurePosition = new Vector2[4] { figures[currentFigureFalling][0], figures[currentFigureFalling][1], figures[currentFigureFalling][2], figures[currentFigureFalling][3] };
            currentFallingFigurePosition[0] += figureStartPos;
            currentFallingFigurePosition[1] += figureStartPos;
            currentFallingFigurePosition[2] += figureStartPos;
            currentFallingFigurePosition[3] += figureStartPos;

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //handle move objects actions
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (isWeCanMoveObject)
                {
                    //move left
                    if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    {
                        currentFallingFigurePosition[0] += DirLeft;
                        currentFallingFigurePosition[1] += DirLeft;
                        currentFallingFigurePosition[2] += DirLeft;
                        currentFallingFigurePosition[3] += DirLeft;


                        //calculate and fix left position
                        double mostLeftPosition = currentFallingFigurePosition[0].X;
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentFallingFigurePosition[i].X < mostLeftPosition)
                            {
                                mostLeftPosition = currentFallingFigurePosition[i].X;
                            }
                        }
                        while (mostLeftPosition < 0)
                        {
                            if (mostLeftPosition < 0)
                            {
                                currentFallingFigurePosition[0] += DirRight;
                                currentFallingFigurePosition[1] += DirRight;
                                currentFallingFigurePosition[2] += DirRight;
                                currentFallingFigurePosition[3] += DirRight;
                            }
                            //find new left position
                            mostLeftPosition = currentFallingFigurePosition[0].X;
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentFallingFigurePosition[i].X < mostLeftPosition)
                                {
                                    mostLeftPosition = currentFallingFigurePosition[i].X;
                                }
                            }
                        }
                                                
                        for (int i = 0; i < fallenSquares.Count; i++)
                        {
                            //check on impact with an existing fallen squares
                            if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                            {
                                currentFallingFigurePosition[0] += DirRight;
                                currentFallingFigurePosition[1] += DirRight;
                                currentFallingFigurePosition[2] += DirRight;
                                currentFallingFigurePosition[3] += DirRight;
                                break;
                            }
                            else {
                                if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                                {
                                    currentFallingFigurePosition[0] += DirRight;
                                    currentFallingFigurePosition[1] += DirRight;
                                    currentFallingFigurePosition[2] += DirRight;
                                    currentFallingFigurePosition[3] += DirRight;
                                    break;
                                }
                            }
                        }
                    }
                    //move right
                    if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    {
                        currentFallingFigurePosition[0] += DirRight;
                        currentFallingFigurePosition[1] += DirRight;
                        currentFallingFigurePosition[2] += DirRight;
                        currentFallingFigurePosition[3] += DirRight;

                        //calculate and fix right position
                        double mostRightPosition = currentFallingFigurePosition[0].X + squareTexture.Width;
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentFallingFigurePosition[i].X + squareTexture.Width > mostRightPosition)
                            {
                                mostRightPosition = currentFallingFigurePosition[i].X + squareTexture.Width;
                            }
                        }
                        while (mostRightPosition > width)
                        {
                            if (mostRightPosition > width)
                            {
                                currentFallingFigurePosition[0] += DirLeft;
                                currentFallingFigurePosition[1] += DirLeft;
                                currentFallingFigurePosition[2] += DirLeft;
                                currentFallingFigurePosition[3] += DirLeft;
                            }
                            //find new right position
                            mostRightPosition = currentFallingFigurePosition[0].X + squareTexture.Width;
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentFallingFigurePosition[i].X + squareTexture.Width > mostRightPosition)
                                {
                                    mostRightPosition = currentFallingFigurePosition[i].X + squareTexture.Width;
                                }
                            }
                        }

                        for (int i = 0; i < fallenSquares.Count; i++)
                        {
                            //check on impact with an existing fallen squares
                            if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                            {
                                currentFallingFigurePosition[0] += DirLeft;
                                currentFallingFigurePosition[1] += DirLeft;
                                currentFallingFigurePosition[2] += DirLeft;
                                currentFallingFigurePosition[3] += DirLeft;
                                break;
                            }
                            else
                            {
                                if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 80 / 100) - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                                {
                                    currentFallingFigurePosition[0] += DirLeft;
                                    currentFallingFigurePosition[1] += DirLeft;
                                    currentFallingFigurePosition[2] += DirLeft;
                                    currentFallingFigurePosition[3] += DirLeft;
                                    break;
                                }
                            }
                        }
                    }
                    //rotate current figure
                    if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    {
                        //calcute next figure by current falling 
                        switch (currentFigureFalling) {
                            case 0:
                                currentFigureFalling = 1;
                                break;
                            case 1:
                                currentFigureFalling = 0;
                                break;
                            case 2:
                                currentFigureFalling = 3;
                                break;
                            case 3:
                                currentFigureFalling = 4;
                                break;
                            case 4:
                                currentFigureFalling = 5;
                                break;
                            case 5:
                                currentFigureFalling = 2;
                                break;
                            case 6:
                                currentFigureFalling = 7;
                                break;
                            case 7:
                                currentFigureFalling = 8;
                                break;
                            case 8:
                                currentFigureFalling = 9;
                                break;
                            case 9:
                                currentFigureFalling = 6;
                                break;
                            case 10:
                                currentFigureFalling = 11;
                                break;
                            case 11:
                                currentFigureFalling = 12;
                                break;
                            case 12:
                                currentFigureFalling = 13;
                                break;
                            case 13:
                                currentFigureFalling = 10;
                                break;
                            case 14:
                                currentFigureFalling = 15;
                                break;
                            case 15:
                                currentFigureFalling = 14;
                                break;
                            case 16:
                                currentFigureFalling = 17;
                                break;
                            case 17:
                                currentFigureFalling = 16;
                                break;
                            case 18:
                                currentFigureFalling = 18;
                                break;

                        };
                        //save old  zero zero position
                        Vector2 oldZeroZero = currentFallingFigurePosition[0];

                        currentFallingFigurePosition[0] = figures[currentFigureFalling][0] + oldZeroZero;
                        currentFallingFigurePosition[1] = figures[currentFigureFalling][1] + currentFallingFigurePosition[0]; 
                        currentFallingFigurePosition[2] = figures[currentFigureFalling][2] + currentFallingFigurePosition[0];
                        currentFallingFigurePosition[3] = figures[currentFigureFalling][3] + currentFallingFigurePosition[0];

                        //calculate and fix left position
                        double mostLeftPosition = currentFallingFigurePosition[0].X;
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentFallingFigurePosition[i].X < mostLeftPosition)
                            {
                                mostLeftPosition = currentFallingFigurePosition[i].X;
                            }
                        }
                        while (mostLeftPosition < 0)
                        {
                            if (mostLeftPosition < 0)
                            {
                                currentFallingFigurePosition[0] += DirRight;
                                currentFallingFigurePosition[1] += DirRight;
                                currentFallingFigurePosition[2] += DirRight;
                                currentFallingFigurePosition[3] += DirRight;
                            }
                            //find new left position
                            mostLeftPosition = currentFallingFigurePosition[0].X;
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentFallingFigurePosition[i].X < mostLeftPosition)
                                {
                                    mostLeftPosition = currentFallingFigurePosition[i].X;
                                }
                            }
                        }

                        //calculate and fix right position
                        double mostRightPosition = currentFallingFigurePosition[0].X + squareTexture.Width;
                        for (int i = 0; i < 4; i++)
                        {
                            if (currentFallingFigurePosition[i].X + squareTexture.Width > mostRightPosition)
                            {
                                mostRightPosition = currentFallingFigurePosition[i].X + squareTexture.Width;
                            }
                        }
                        while (mostRightPosition > width)
                        {
                            if (mostRightPosition > width)
                            {
                                currentFallingFigurePosition[0] += DirLeft;
                                currentFallingFigurePosition[1] += DirLeft;
                                currentFallingFigurePosition[2] += DirLeft;
                                currentFallingFigurePosition[3] += DirLeft;
                            }
                            //find new right position
                            mostRightPosition = currentFallingFigurePosition[0].X + squareTexture.Width;
                            for (int i = 0; i < 4; i++)
                            {
                                if (currentFallingFigurePosition[i].X + squareTexture.Width > mostRightPosition)
                                {
                                    mostRightPosition = currentFallingFigurePosition[i].X + squareTexture.Width;
                                }
                            }
                        }

                        for (int i = 0; i < fallenSquares.Count; i++)
                        {
                            //check on impact with an existing fallen squares
                            if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                            {
                                //calcute prev figure by current falling 
                                switch (currentFigureFalling)
                                {
                                    case 1:
                                        currentFigureFalling = 0;
                                        break;
                                    case 0:
                                        currentFigureFalling = 1;
                                        break;
                                    case 3:
                                        currentFigureFalling = 2;
                                        break;
                                    case 4:
                                        currentFigureFalling = 3;
                                        break;
                                    case 5:
                                        currentFigureFalling = 4;
                                        break;
                                    case 2:
                                        currentFigureFalling = 5;
                                        break;
                                    case 7:
                                        currentFigureFalling = 6;
                                        break;
                                    case 8:
                                        currentFigureFalling = 7;
                                        break;
                                    case 9:
                                        currentFigureFalling = 8;
                                        break;
                                    case 6:
                                        currentFigureFalling = 9;
                                        break;
                                    case 11:
                                        currentFigureFalling = 10;
                                        break;
                                    case 12:
                                        currentFigureFalling = 11;
                                        break;
                                    case 13:
                                        currentFigureFalling = 12;
                                        break;
                                    case 10:
                                        currentFigureFalling = 13;
                                        break;
                                    case 15:
                                        currentFigureFalling = 14;
                                        break;
                                    case 14:
                                        currentFigureFalling = 15;
                                        break;
                                    case 17:
                                        currentFigureFalling = 16;
                                        break;
                                    case 16:
                                        currentFigureFalling = 17;
                                        break;
                                    case 18:
                                        currentFigureFalling = 18;
                                        break;

                                };
                                currentFallingFigurePosition[0] = figures[currentFigureFalling][0] + oldZeroZero;
                                currentFallingFigurePosition[1] = figures[currentFigureFalling][1] + currentFallingFigurePosition[0];
                                currentFallingFigurePosition[2] = figures[currentFigureFalling][2] + currentFallingFigurePosition[0];
                                currentFallingFigurePosition[3] = figures[currentFigureFalling][3] + currentFallingFigurePosition[0];
                                break;
                            }
                            else
                            {
                                if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[0].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[0].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[1].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[1].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[2].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[2].Y < squareTexture.Height)) ||
                                    ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[3].Y > 0) && (fallenSquares[i].Y + (squareTexture.Height * 100 / 100) - currentFallingFigurePosition[3].Y < squareTexture.Height)))
                                {
                                    //calcute prev figure by current falling 
                                    switch (currentFigureFalling)
                                    {
                                        case 1:
                                            currentFigureFalling = 0;
                                            break;
                                        case 0:
                                            currentFigureFalling = 1;
                                            break;
                                        case 3:
                                            currentFigureFalling = 2;
                                            break;
                                        case 4:
                                            currentFigureFalling = 3;
                                            break;
                                        case 5:
                                            currentFigureFalling = 4;
                                            break;
                                        case 2:
                                            currentFigureFalling = 5;
                                            break;
                                        case 7:
                                            currentFigureFalling = 6;
                                            break;
                                        case 8:
                                            currentFigureFalling = 7;
                                            break;
                                        case 9:
                                            currentFigureFalling = 8;
                                            break;
                                        case 6:
                                            currentFigureFalling = 9;
                                            break;
                                        case 11:
                                            currentFigureFalling = 10;
                                            break;
                                        case 12:
                                            currentFigureFalling = 11;
                                            break;
                                        case 13:
                                            currentFigureFalling = 12;
                                            break;
                                        case 10:
                                            currentFigureFalling = 13;
                                            break;
                                        case 15:
                                            currentFigureFalling = 14;
                                            break;
                                        case 14:
                                            currentFigureFalling = 15;
                                            break;
                                        case 17:
                                            currentFigureFalling = 16;
                                            break;
                                        case 16:
                                            currentFigureFalling = 17;
                                            break;
                                        case 18:
                                            currentFigureFalling = 18;
                                            break;

                                    };
                                    currentFallingFigurePosition[0] = figures[currentFigureFalling][0] + oldZeroZero;
                                    currentFallingFigurePosition[1] = figures[currentFigureFalling][1] + currentFallingFigurePosition[0];
                                    currentFallingFigurePosition[2] = figures[currentFigureFalling][2] + currentFallingFigurePosition[0];
                                    currentFallingFigurePosition[3] = figures[currentFigureFalling][3] + currentFallingFigurePosition[0];
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                { 
                    if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    {
                        currentFallingFigurePosition[0] += DirDownFast;
                        currentFallingFigurePosition[1] += DirDownFast;
                        currentFallingFigurePosition[2] += DirDownFast;
                        currentFallingFigurePosition[3] += DirDownFast;
                    }
                    else
                    {
                        currentFallingFigurePosition[0] += DirDown;
                        currentFallingFigurePosition[1] += DirDown;
                        currentFallingFigurePosition[2] += DirDown;
                        currentFallingFigurePosition[3] += DirDown;
                    }
                }
                isWeCanMoveObject = false;
            }
            else
            {
                isWeCanMoveObject = true;
                //move to new pos position our simple square
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    currentFallingFigurePosition[0] += DirDownFast;
                    currentFallingFigurePosition[1] += DirDownFast;
                    currentFallingFigurePosition[2] += DirDownFast;
                    currentFallingFigurePosition[3] += DirDownFast;
                }
                else
                {
                    currentFallingFigurePosition[0] += DirDown;
                    currentFallingFigurePosition[1] += DirDown;
                    currentFallingFigurePosition[2] += DirDown;
                    currentFallingFigurePosition[3] += DirDown;
                }
            }

            //calculate free height all of x Game Lines
            int[] topElement = new int[xGameSize]; 

            for (int i = 0; i < xGameSize; i++) {
                topElement[i] = height;
            }
            for (int i = 0; i < fallenSquares.Count; i++)
            {

                if (((int)fallenSquares[i].Y < topElement[(int)fallenSquares[i].X / squareTexture.Width]))
                {
                    if (((fallenSquares[i].X == currentFallingFigurePosition[0].X) && (fallenSquares[i].Y > currentFallingFigurePosition[0].Y)) ||
                        ((fallenSquares[i].X == currentFallingFigurePosition[1].X) && (fallenSquares[i].Y > currentFallingFigurePosition[1].Y)) ||
                        ((fallenSquares[i].X == currentFallingFigurePosition[2].X) && (fallenSquares[i].Y > currentFallingFigurePosition[2].Y)) ||
                        ((fallenSquares[i].X == currentFallingFigurePosition[3].X) && (fallenSquares[i].Y > currentFallingFigurePosition[3].Y)))
                    {
                        topElement[(int)fallenSquares[i].X / squareTexture.Width] = (int)fallenSquares[i].Y;
                    }
                }
            }
            
            //in cycle check per square on impact
            for (int i = 0; i < 4; i++)
            {
                //fix position
                while (currentFallingFigurePosition[i].Y + squareTexture.Height > topElement[(int)currentFallingFigurePosition[i].X / squareTexture.Width])
                {
                    currentFallingFigurePosition[0] += DirTop;
                    currentFallingFigurePosition[1] += DirTop;
                    currentFallingFigurePosition[2] += DirTop;
                    currentFallingFigurePosition[3] += DirTop;
                }
                if (currentFallingFigurePosition[i].Y + squareTexture.Height == topElement[(int)currentFallingFigurePosition[i].X / squareTexture.Width]) {
                    //figure is fallen on the bottom of game size, save current figure to fallen squares, drop new figure
                    for (int c = 0; c < 4; c++)
                    {
                        fallenSquares.Add(new Vector2(currentFallingFigurePosition[c].X, currentFallingFigurePosition[c].Y));
                    }

                    //try to check which lines we can erase
                    int[] SquaresInLine = new int[yGameSize];
                    //init array
                    for (int ki = 0; ki < yGameSize; ki++)
                    {
                        SquaresInLine[i] = 0;
                    }
                    //calculate current squares in lines
                    for (int c = 0; c < fallenSquares.Count; c++)
                    {
                        int line = ((int)fallenSquares[c].Y / squareTexture.Width) - 1;
                        //check for game over
                        if (line >= yGameSize || line < 0)
                        {
                            Exit();
                            return;
                        }
                        //increase counter of squares in line
                        SquaresInLine[line]++;
                    }
                    //erase squares
                    int tempScoreKoef = 0;
                    for (int k = 0; k < yGameSize; k++)
                    {
                        if (SquaresInLine[k] >= xGameSize)
                        {
                            tempScoreKoef++;
                            //clean line 
                            for (int c = fallenSquares.Count-1; c >=0; c--)
                            {
                                if ((k + 1) * squareTexture.Width == (int)fallenSquares[c].Y) { 
                                    fallenSquares.RemoveAt(c);
                                }
                            }
                            //and drop down topper items
                            for (int c = fallenSquares.Count - 1; c >= 0; c--)
                            {
                                if ((k + 1) * squareTexture.Width > (int)fallenSquares[c].Y)
                                {
                                    fallenSquares[c] += new Vector2(0, squareTexture.Width);
                                } 
                            }
                        }
                    }

                    //add scores
                    if (tempScoreKoef > 0)
                    {
                        int tempScore = 0;
                        tempScore = 100 * tempScoreKoef;
                        score += tempScore + tempScore - 100;
                    }

                    //drop newt figure
                    currentFigureFalling = (new Random(DateTime.Now.Millisecond)).Next(figures.Count);
                    currentFallingFigurePosition = new Vector2[4] { figures[currentFigureFalling][0], figures[currentFigureFalling][1], figures[currentFigureFalling][2], figures[currentFigureFalling][3] };
                    currentFallingFigurePosition[0] += figureStartPos;
                    currentFallingFigurePosition[1] += figureStartPos;
                    currentFallingFigurePosition[2] += figureStartPos;
                    currentFallingFigurePosition[3] += figureStartPos;
                    //exit from cycle
                    break;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            //begin sprite batch
            spriteBatch.Begin();

            //draw game infot
            spriteBatch.DrawString(font, "Score: "+Convert.ToString(score), new Vector2(10, 10), Color.Black);

            //draw current falling figure
            spriteBatch.Draw(squareTexture, currentFallingFigurePosition[0], Color.White);
            spriteBatch.Draw(squareTexture, currentFallingFigurePosition[1], Color.White);
            spriteBatch.Draw(squareTexture, currentFallingFigurePosition[2], Color.White);
            spriteBatch.Draw(squareTexture, currentFallingFigurePosition[3], Color.White);

            //draw fallen squares
            for (int i = 0; i < fallenSquares.Count; i++)
            {
                spriteBatch.Draw(squareTexture, fallenSquares[i], Color.White);
            }
            //end sprite batch
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
