using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Off_brand_Minecraft
{
    internal class Player
    {
        //värld
        public bool playerIsInWorld = false;
        public short worldWithPlayer = 0;

        //perspektiv
        public double[] playerPos = new double[3];
        public double[] playerPerspective = new double[2];
        //playerPerspective 0 = vinkeln från X-axeln i XZ-planet
        //playerPerspective 1 = vinkeln från X-axeln i XY-planet
        //playerPerspective 2 = vinkeln från Z-axeln i YZ-planet

        //inventory
        public short[,] hotBar = new short[10, 2]; // [n, 0] är blockens koder, [n, 1] är antalet block i rutan
        public short[,,] inventory = new short[10, 3, 2]; // [a, b, 0] är blockens koder, [a, b, 1] är antalet block i rutan
        public Label[,] inventoryAmountIndicators = new Label[10, 3];
        public Label[] hotBarAmountIndicators = new Label[10];
        public short[] pickedUpBlock = new short[2];
        public int hotBarTarget = 0;
        public bool inventoryIsOpen = false;
        public short heldBlock = 0;
        short stackSize = 64;

        //rörelse
        public bool[] possibleDirections = new bool[6];
        public int[] distanceToStop = new int[6];
        public int speed = 5;
        public int walkSpeed = 5;
        public int sprintSpeed = 10;
        public int verticalspeed = 0;
        public int jumpSpeed = 15;
        public int gravity = 2;
        public bool walk;
        public bool[] controlInputs = new bool[4];
        public int hitBox = 10;
        public float walkingDirection = 0;
        //Direction 0 X+
        //Direction 1 X-
        //Direction 2 Y+
        //Direction 3 Y-
        //Direction 4 Z+
        //Direction 5 Z-

        //världinteraktioner
        public bool playerIsDestorying = false;
        public int[] targetedSurface = new int[4];
        public void CheckCollisions(ref bool[] possibleDirections, short[,,] world, double[] playerPos, int hitBox, ref int[] distanceToStop)
        {
            for(int i = 0; i < possibleDirections.Length; i++) possibleDirections[i] = true;
            for (int i = 0; i < speed; i++)
            {
                for (float k = 0; k < 1.5; k += 0.1f)
                {
                    try
                    {
                        if (possibleDirections[1] && (
                            world[(int)((playerPos[0] + hitBox + i) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                            world[(int)((playerPos[0] + hitBox + i) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] - hitBox + 1) / 30)] != 0)) //spelaren kan inte röra sig i X+
                        {
                            distanceToStop[1] = i;
                            possibleDirections[1] = false;
                        }
                        if (possibleDirections[0] && (
                            world[(int)((playerPos[0] - hitBox - i) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                            world[(int)((playerPos[0] - hitBox - i) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] - hitBox + 1) / 30)] != 0)) //spelaren kan inte röra sig i X-
                        {
                            distanceToStop[0] = i;
                            possibleDirections[0] = false;
                        }
                        if (possibleDirections[5] && (
                            world[(int)((playerPos[0] + hitBox - 1) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] + hitBox + i) / 30)] != 0 ||
                            world[(int)((playerPos[0] - hitBox + 1) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] + hitBox + i) / 30)] != 0)) //spelaren kan inte röra sig i Z+
                        {
                            distanceToStop[5] = i;
                            possibleDirections[5] = false;
                        }
                        if (possibleDirections[4] && (
                            world[(int)((playerPos[0] + hitBox - 1) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] - hitBox - i) / 30)] != 0 ||
                            world[(int)((playerPos[0] - hitBox + 1) / 30), (int)(playerPos[1] / 30 - k), (int)((playerPos[2] - hitBox - i) / 30)] != 0)) //spelaren kan inte röra sig i Z-
                        {
                            distanceToStop[4] = i;
                            possibleDirections[4] = false;
                        }
                    }
                    catch
                    {

                    }
                }    
            }
            int j = 0;
            do
            {
                try
                {
                    if (possibleDirections[3] && (
                        world[(int)((playerPos[0] + hitBox - 1) / 30), (int)((playerPos[1] - j - 15) / 30 - 1), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] + hitBox - 1) / 30), (int)((playerPos[1] - j - 15) / 30 - 1), (int)((playerPos[2] - hitBox + 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] - hitBox + 1) / 30), (int)((playerPos[1] - j - 15) / 30 - 1), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] - hitBox + 1) / 30), (int)((playerPos[1] - j - 15) / 30 - 1), (int)((playerPos[2] - hitBox + 1) / 30)] != 0)) //spelaren kan inte röra sig i Y-
                    {
                        distanceToStop[3] = j;
                        possibleDirections[3] = false;
                    }
                }
                catch { }
                j++;
            }
            while (j <= -verticalspeed);
            j = 0;
            do
            {
                try
                {
                    if (possibleDirections[2] && (
                        world[(int)((playerPos[0] + hitBox - 1) / 30), (int)((playerPos[1] + j) / 30), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] + hitBox - 1) / 30), (int)((playerPos[1] + j) / 30), (int)((playerPos[2] - hitBox + 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] - hitBox + 1) / 30), (int)((playerPos[1] + j) / 30), (int)((playerPos[2] + hitBox - 1) / 30)] != 0 ||
                        world[(int)((playerPos[0] - hitBox + 1) / 30), (int)((playerPos[1] + j) / 30), (int)((playerPos[2] - hitBox + 1) / 30)] != 0)) //spelaren kan inte röra sig i Y+
                    {
                        distanceToStop[2] = j;
                        possibleDirections[2] = false;
                    }
                }
                catch { }
                j++;

            }
            while (j <= verticalspeed);
        }
        public void ChooseDirectionAndWalk(ref double[] playerPos, bool[] possibleDirections, double playerAngle, float proposedDirection)
        {
            double xSpeed = Math.Cos(proposedDirection * Math.PI / 2 + playerAngle) * speed; //flytta spelaren proportionerligt utefter perspektiv
            double zSpeed = Math.Sin(proposedDirection * Math.PI / 2 + playerAngle) * speed;
            if(xSpeed < 0)
            {
                if (possibleDirections[0]) playerPos[0] += xSpeed; //flytta spelaren
                else playerPos[0] -= distanceToStop[0]; //gå emot kant på grund av begränsad sträcka till hinder
            }
            if(xSpeed > 0)
            {
                if (possibleDirections[1]) playerPos[0] += xSpeed;
                else playerPos[0] += distanceToStop[1];
            }
            if(zSpeed < 0)
            {
                if (possibleDirections[4]) playerPos[2] += zSpeed;
                else playerPos[2] -= distanceToStop[4];
            }
            if(zSpeed > 0)
            {
                if (possibleDirections[5]) playerPos[2] += zSpeed;
                else playerPos[2] += distanceToStop[5];
            }
        }
        public void PlayerPlaceBlock(ref short[,,] blocks, ref short[,,] destructionLevels, ref bool[,,] blockPowering)
        {
            if(heldBlock != 0) //placera block om spelaren håller i ett block
            {
                try
                {
                    int[] blockToPlace = new int[3];
                    for (int i = 0; i < 3; i++) //bestäm var blocket ska plaveras
                    {
                        if (targetedSurface[3] == 2 * i) blockToPlace[i] = targetedSurface[i] + 1;
                        else if (targetedSurface[3] == 2 * i + 1) blockToPlace[i] = targetedSurface[i] - 1;
                        else blockToPlace[i] = targetedSurface[i];
                    }
                    if (((int)(playerPos[0] - hitBox + 1) / 30 == blockToPlace[0] || (int)(playerPos[0] + hitBox - 1) / 30 == blockToPlace[0]) &&
                        ((int)(playerPos[1] - 14) / 30 - 1 == blockToPlace[1] || (int)(playerPos[1] - 15) / 30 == blockToPlace[1] || (int)(playerPos[1] + 15) / 30 == blockToPlace[1]) &&
                        ((int)(playerPos[2] - hitBox + 1) / 30 == blockToPlace[2] || (int)(playerPos[2] + hitBox - 1) / 30 == blockToPlace[2])) //bestäm om blocket kan placeras i förhållande till spelarens hitbox
                    { }
                    else
                    {
                        blocks[blockToPlace[0], blockToPlace[1], blockToPlace[2]] = hotBar[hotBarTarget, 0];
                        destructionLevels[blockToPlace[0], blockToPlace[1], blockToPlace[2]] = 100;
                        hotBar[hotBarTarget, 1]--;
                        if (hotBar[hotBarTarget, 1] == 0) hotBar[hotBarTarget, 0] = 0;
                    }
                }
                catch { }
            }
            else if (blocks[targetedSurface[0], targetedSurface[1], targetedSurface[2]] == 5)
            {
                blockPowering[targetedSurface[0], targetedSurface[1], targetedSurface[2]] = !blockPowering[targetedSurface[0], targetedSurface[1], targetedSurface[2]];
            }
        }
        public void DrawGUI(Graphics g, Size screen, Point cursor)
        {
            Pen pen = new Pen(Color.Black, 3);
            SolidBrush[] brushes = new SolidBrush[8];
            brushes[0] = new SolidBrush(Color.Yellow);
            brushes[1] = new SolidBrush(Color.Black);
            brushes[2] = new SolidBrush(Color.Gray);
            brushes[3] = new SolidBrush(Color.SaddleBrown);
            brushes[4] = new SolidBrush(Color.Green);
            brushes[5] = new SolidBrush(Color.Blue);
            brushes[6] = new SolidBrush(Color.FromArgb(145, 82, 45));
            brushes[7] = new SolidBrush(Color.LawnGreen);

            for (int i = 0; i < 10; i++)
            {
                Point[] hotBarSlot = {    new Point(screen.Width / 20 * i + screen.Width / 4, 9 * screen.Height / 10 - screen.Width / 20),      //X-Min = screen.Width / 4, X-Max = 3 * screen.Width / 4
                                           new Point(screen.Width / 20 * i + 6 * screen.Width / 20, 9 * screen.Height / 10 - screen.Width / 20),//Y-Min = screen.Height / 10 - screen.Width / 20, Y-Max = screen.Height / 10
                                           new Point(screen.Width / 20 * i + 6 * screen.Width / 20, 9 * screen.Height / 10), 
                                           new Point(screen.Width / 20 * i + screen.Width / 4,  9 * screen.Height / 10)};
                g.DrawPolygon(pen, hotBarSlot);
                if (hotBar[i, 0] != 0) //rita blocket som befinner sig i en hotbarruta
                {
                    int width = screen.Width / 40;
                    Point[] displayTopFacet = new Point[4];
                    Point[] displayLeftFacet = new Point[4];
                    Point[] displayRightFacet = new Point[4];
                    DisplayBlockInSLot(ref displayTopFacet, ref displayRightFacet, ref displayLeftFacet, width, hotBarSlot);
                    if (hotBar[i,0] == 4)
                    {
                        g.FillPolygon(brushes[4], displayTopFacet);
                        g.FillPolygon(brushes[3], displayRightFacet);
                        g.FillPolygon(brushes[3], displayLeftFacet);
                    }
                    else
                    {
                        g.FillPolygon(brushes[hotBar[i, 0]], displayTopFacet);
                        g.FillPolygon(brushes[hotBar[i, 0]], displayRightFacet);
                        g.FillPolygon(brushes[hotBar[i, 0]], displayLeftFacet);
                    }
                    g.DrawPolygon(Pens.Black, displayTopFacet);
                    g.DrawPolygon(Pens.Black, displayRightFacet);
                    g.DrawPolygon(Pens.Black, displayLeftFacet);
                }
            }
            if (inventoryIsOpen) //rita inventariets innehåll
            {
                g.FillRectangle(Brushes.DarkGray, 3 * screen.Width / 16 - screen.Width / 40, 3 * screen.Height / 4 - 3 * screen.Width / 16 - screen.Width / 40, 10 * screen.Width / 16 + screen.Width / 20, 3 * screen.Width / 16 + screen.Width / 20); //rita inventory
                g.DrawRectangle(pen, 3 * screen.Width / 16 - screen.Width / 40, 3 * screen.Height / 4 - 3 * screen.Width / 16 - screen.Width / 40, 10 * screen.Width / 16 + screen.Width / 20, 3 * screen.Width / 16 + screen.Width / 20);

                g.FillRectangle(Brushes.DarkGray, 2 * screen.Width / 16 - screen.Width / 40, screen.Height / 2 - screen.Width / 40, screen.Width / 16, screen.Width / 16); //rita bakgrund för raderingsruta
                g.DrawRectangle(pen, 2 * screen.Width / 16 - screen.Width / 40, screen.Height / 2 - screen.Width / 40, screen.Width / 16, screen.Width / 16);

                g.FillRectangle(Brushes.Red, 2 * screen.Width / 16 - screen.Width / 60, screen.Height / 2 - screen.Width / 60, screen.Width / 16 - screen.Width / 60, screen.Width / 16 - screen.Width / 60); //rita raderingsruta
                g.DrawRectangle(pen, 2 * screen.Width / 16 - screen.Width / 60, screen.Height / 2 - screen.Width / 60, screen.Width / 16 - screen.Width / 60, screen.Width / 16 - screen.Width / 60);
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Point[] inventorySlot = {   new Point(screen.Width / 16 * i + 3 * screen.Width / 16, 3 * screen.Height / 4 - screen.Width / 16 * (j + 1)),      //X-Min = 3 * screen.Width / 16, X-Max = 13 * screen.Width / 16
                                                    new Point(screen.Width / 16 * (i + 1) + 3 * screen.Width / 16, 3 * screen.Height / 4 - screen.Width / 16 * (j + 1)),//Y-Min = 3 * screen.Height / 4 - screen.Width / 16, Y-Max = 3 * screen.Height / 4
                                                    new Point(screen.Width / 16 * (i + 1) + 3 * screen.Width / 16, 3 * screen.Height / 4 - screen.Width / 16 * j),      //rutdimensioner: screen.Width / 16 X screen.Width / 16
                                                    new Point(screen.Width / 16 * i + 3 * screen.Width / 16, 3 * screen.Height / 4 - screen.Width / 16 * j) };
                        g.DrawPolygon(pen, inventorySlot);
                        if (inventory[i, j, 0] != 0)
                        {
                            int width = screen.Width / 32;
                            Point[] displayTopFacet = new Point[4];
                            Point[] displayLeftFacet = new Point[4];
                            Point[] displayRightFacet = new Point[4];
                            DisplayBlockInSLot(ref displayTopFacet, ref displayRightFacet, ref displayLeftFacet, width, inventorySlot);
                            if (inventory[i, j, 0] == 4)
                            {
                                g.FillPolygon(brushes[4], displayTopFacet);
                                g.FillPolygon(brushes[3], displayRightFacet);
                                g.FillPolygon(brushes[3], displayLeftFacet);
                            }
                            else
                            {
                                g.FillPolygon(brushes[inventory[i, j, 0]], displayTopFacet);
                                g.FillPolygon(brushes[inventory[i, j, 0]], displayRightFacet);
                                g.FillPolygon(brushes[inventory[i, j, 0]], displayLeftFacet);
                            }
                            g.DrawPolygon(Pens.Black, displayTopFacet);
                            g.DrawPolygon(Pens.Black, displayRightFacet);
                            g.DrawPolygon(Pens.Black, displayLeftFacet);
                        }
                    }
                }
            }
            Point[] hotBarTargetSquare = {  new Point(screen.Width / 20 * hotBarTarget + screen.Width / 4, 9 * screen.Height / 10 - screen.Width / 20),
                                            new Point(screen.Width / 20 * hotBarTarget + 6 * screen.Width / 20, 9 * screen.Height / 10 - screen.Width / 20), 
                                            new Point(screen.Width / 20 * hotBarTarget + 6 * screen.Width / 20, 9 * screen.Height / 10), 
                                            new Point(screen.Width / 20 * hotBarTarget + screen.Width / 4, 9 * screen.Height / 10) };
            pen.Color = Color.White;
            pen.Width = 7;
            g.DrawPolygon(pen, hotBarTargetSquare);
            if (pickedUpBlock[0] != 0)
            {
                Point[] displayTopFacet = new Point[4];
                Point[] displayLeftFacet = new Point[4];
                Point[] displayRightFacet = new Point[4];
                Point[] heldPosition = {    cursor, 
                                            new Point(cursor.X + screen.Width / 20, cursor.Y), 
                                            new Point(cursor.X + screen.Width / 20, cursor.Y + screen.Width / 20), 
                                            new Point(cursor.X, cursor.Y + screen.Width / 20) };
                DisplayBlockInSLot(ref displayTopFacet, ref displayLeftFacet, ref displayRightFacet, screen.Width / 40, heldPosition);
                if (pickedUpBlock[0] == 4)
                {
                    g.FillPolygon(brushes[4], displayTopFacet);
                    g.FillPolygon(brushes[3], displayRightFacet);
                    g.FillPolygon(brushes[3], displayLeftFacet);
                }
                else
                {
                    g.FillPolygon(brushes[pickedUpBlock[0]], displayTopFacet);
                    g.FillPolygon(brushes[pickedUpBlock[0]], displayRightFacet);
                    g.FillPolygon(brushes[pickedUpBlock[0]], displayLeftFacet);
                }
                g.DrawPolygon(Pens.Black, displayTopFacet);
                g.DrawPolygon(Pens.Black, displayRightFacet);
                g.DrawPolygon(Pens.Black, displayLeftFacet);
            }
        }
        static void DisplayBlockInSLot(ref Point[] displayTopFacet, ref Point[] displayRightFacet, ref Point[] displayLeftFacet, int width, Point[] inventorySlot) //specifika koordinater för att projicera ett block i en ruta
        {
            displayTopFacet[0] = new Point(inventorySlot[0].X + width / 4, inventorySlot[0].Y + width / 2);
            displayTopFacet[1] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + width / 8);
            displayTopFacet[2] = new Point(inventorySlot[0].X + 7 * width / 4, inventorySlot[0].Y + width / 2);
            displayTopFacet[3] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + 7 * width / 8);
            displayLeftFacet[0] = new Point(inventorySlot[0].X + width / 4, inventorySlot[0].Y + width / 2);
            displayLeftFacet[1] = new Point(inventorySlot[0].X + width / 4, inventorySlot[0].Y + 4 * width / 3);
            displayLeftFacet[2] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + 15 * width / 8);
            displayLeftFacet[3] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + 7 * width / 8);
            displayRightFacet[0] = new Point(inventorySlot[0].X + 7 * width / 4, inventorySlot[0].Y + width / 2);
            displayRightFacet[1] = new Point(inventorySlot[0].X + 7 * width / 4, inventorySlot[0].Y + 4 * width / 3);
            displayRightFacet[2] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + 15 * width / 8);
            displayRightFacet[3] = new Point(inventorySlot[0].X + width, inventorySlot[0].Y + 7 * width / 8);
        }
        public void PickUpSlot(ref short[] pickedStack, Point cursor, Size screen) //plocka upp en grupp block
        {
            if(cursor.X > 3 * screen.Width / 16 && cursor.X < 13 * screen.Width / 16 && cursor.Y > 3 * screen.Height / 4 - 3 * screen.Width / 16 && cursor.Y < 3 * screen.Height / 4)
            {
                if (inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 0] == pickedStack[0])
                {
                    if (inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1] + pickedStack[1] <= stackSize)
                    {
                        inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1] += pickedStack[1];
                        pickedStack[0] = 0;
                        pickedStack[1] = 0;
                        RefreshInventoryAmountIndicatorValues();
                    }
                    else
                    {
                        pickedStack[1] -= (short)(stackSize - inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1]);
                        inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1] = stackSize;
                        RefreshInventoryAmountIndicatorValues();
                    }
                }
                else
                {
                    short[] savedStack = new short[2];
                    for (int i = 0; i < 2; i++)
                    {
                        savedStack[i] = inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, i];
                        inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, i] = pickedStack[i];
                        pickedStack[i] = savedStack[i];
                        RefreshInventoryAmountIndicatorValues();

                    }
                }
            }
            else if (cursor.X > screen.Width / 4 && cursor.X < 3 * screen.Width / 4 && cursor.Y > 9 * screen.Height / 10 - screen.Width / 20 && cursor.Y < 9 * screen.Height / 10)
            {
                if (hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 0] == pickedStack[0])
                {
                    if(hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1] + pickedStack[1] <= stackSize)
                    {
                        hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1] += pickedStack[1];
                        pickedStack[0] = 0;
                        pickedStack[1] = 0;
                        RefreshHotBarAmountIndicatorValues();
                    }
                    else
                    {
                        pickedStack[1] -= (short)(stackSize - hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1]);
                        hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1] = stackSize;
                        RefreshHotBarAmountIndicatorValues();
                    }
                }
                else
                {
                    short[] savedStack = new short[2];
                    for (int i = 0; i < 2; i++)
                    {
                        savedStack[i] = hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, i];
                        hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, i] = pickedStack[i];
                        pickedStack[i] = savedStack[i];
                        RefreshHotBarAmountIndicatorValues();

                    }
                }      
            }
            else if (cursor.X > 2 * screen.Width / 16 - screen.Width / 60 && cursor.Y > screen.Height / 2 - screen.Width / 60 && cursor.X < 3 * screen.Width / 16 - screen.Width / 60 && cursor.Y < 9 * screen.Height / 16 - screen.Width / 60) //om pekaren är över raderingsrutan raderas det som pekaren håller i
            {
                for (int i = 0; i < 2; i++) pickedStack[i] = 0;
            }
        }
        public void PickUpHalf(ref short[] pickedStack, Point cursor, Size screen) //plocka upp hälften av blocken
        {
            if (pickedStack[1] == 0)
            {
                if (cursor.X > 3 * screen.Width / 16 && cursor.X < 13 * screen.Width / 16 && cursor.Y > 3 * screen.Height / 4 - 3 * screen.Width / 16 && cursor.Y < 3 * screen.Height / 4)
                {
                    short[] savedStack = new short[2];
                    pickedStack[0] = inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 0];
                    savedStack[1] = inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1];
                    inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1] /= 2;
                    pickedStack[1] = (short)(savedStack[1] - inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1]);
                    if (inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 1] == 0)
                    {
                        inventory[(cursor.X - 3 * screen.Width / 16) * 16 / screen.Width, 2 - (cursor.Y - 3 * screen.Height / 4 + 3 * screen.Width / 16) * 16 / screen.Width, 0] = 0;
                    }
                    RefreshInventoryAmountIndicatorValues();
                }
                else if (cursor.X > screen.Width / 4 && cursor.X < 3 * screen.Width / 4 && cursor.Y > 9 * screen.Height / 10 - screen.Width / 20 && cursor.Y < 9 * screen.Height / 10)
                {
                    short[] savedStack = new short[2];
                    pickedStack[0] = hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 0];
                    savedStack[1] = hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1];
                    hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1] /= 2;
                    pickedStack[1] = (short)(savedStack[1] - hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1]);
                    if (hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 1] == 0)
                    {
                        hotBar[(cursor.X - screen.Width / 4) * 20 / screen.Width, 0] = 0;
                    }
                        RefreshHotBarAmountIndicatorValues();
                }
            }
        }
        public void OrderInventoryAmountIndicators(Size screen) //ge alla labels till inventariet rätt koordinater
        {
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 3; j++)
                {
                    inventoryAmountIndicators[i, j] = new Label();
                    inventoryAmountIndicators[i, j].Location = new Point(screen.Width / 16 * i + 3 * screen.Width / 16 + 2, 3 * screen.Height / 4 - screen.Width / 16 * (j + 1) + 2);
                    inventoryAmountIndicators[i, j].BackColor = Color.DarkGray;
                    inventoryAmountIndicators[i, j].Visible = false;
                    inventoryAmountIndicators[i, j].Font = new Font("Arial", 9);
                }
            }
        }
        public void RefreshInventoryAmountIndicatorValues() //ge alla labels till inventariet rätt värde
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(inventoryAmountIndicators[i, j] != null)
                    {
                        if (inventory[i, j, 1] > 1)
                        {
                            inventoryAmountIndicators[i, j].Visible = true;
                            inventoryAmountIndicators[i, j].Text = inventory[i, j, 1].ToString();
                            inventoryAmountIndicators[i, j].AutoSize = true;
                        }
                        else
                        {
                            inventoryAmountIndicators[i, j].Visible = false; //om antalet block är 1 eller 0 visas ej siffran
                        }
                    }
                }
            }
        }
        
        public void HideInventoryAmountIndicators() //göm alla labels till inventariet
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (inventoryAmountIndicators[i, j] != null)
                    {
                        inventoryAmountIndicators[i, j].Visible = false;
                    }
                }
            }
        }
        public void OrderHotBarAmountIndicators(Size screen) //ge alla labels till hotbaren rätt koordinater
        {
            for (int i = 0; i < 10; i++)
            {
                    hotBarAmountIndicators[i] = new Label();
                    hotBarAmountIndicators[i].Location = new Point(screen.Width / 20 * i + screen.Width / 4 + 2, 9 * screen.Height / 10 - screen.Width / 20 + 2);
                    hotBarAmountIndicators[i].BackColor = Color.DarkGray;
                    hotBarAmountIndicators[i].Visible = false;
                    hotBarAmountIndicators[i].Font = new Font("Arial", 9);
            }
        }
        public void RefreshHotBarAmountIndicatorValues() //ge alla labels till hotbaren rätt värden
        {
            for (int i = 0; i < 10; i++)
            {
                if (hotBarAmountIndicators[i] != null)
                {
                    if (hotBar[i, 1] > 1)
                    {
                        hotBarAmountIndicators[i].Visible = true;
                        hotBarAmountIndicators[i].Text = hotBar[i, 1].ToString();
                        hotBarAmountIndicators[i].AutoSize = true;
                    }
                    else
                    {
                        hotBarAmountIndicators[i].Visible = false; //om antalet block är 1 eller 0 visas ej siffran
                    }
                }
            }
        }

        public void HideHotBarAmountIndicators() //göm alla labels till hotbaren
        {
            for (int i = 0; i < 10; i++)
            {
                if (hotBarAmountIndicators[i] != null)
                {
                    hotBarAmountIndicators[i].Visible = false;
                }
            }
        }
        public void PickUpDestroyedBlock(int blockCode) //uppdatera inventariet så att ett förstört block läggs till i spelarens lagring
        {
            bool blockCollected = false;
            for(int i = 0; i < 10; i++)
            {
                if (hotBar[i, 0] == blockCode && hotBar[i, 1] < 64)
                {
                    hotBar[i, 1]++;
                    break;
                }
                for(int j = 2; j >= 0; j--)
                {
                    if (inventory[i, j, 0] == blockCode && inventory[i, j, 1] < 64)
                    {
                        inventory[i, j, 1]++;
                        blockCollected = true;
                        break;
                    }
                    if (i == 9 && j == 2)
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            if (hotBar[k, 0] == 0)
                            {
                                hotBar[k, 0] = (short)blockCode;
                                hotBar[k, 1]++;
                                break;
                            }
                            for (int l = 2; l >= 0; l--)
                            {
                                if (inventory[k, l, 0] == 0)
                                {
                                    inventory[k, l, 1]++;
                                    inventory[k, l, 0] = (short)blockCode;
                                    blockCollected = true;
                                    break;
                                }
                            }
                            if (blockCollected) break;
                        }        
                    }
                }
                if (blockCollected) break;
            }
        }
    }
}
