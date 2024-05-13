using Microsoft.VisualBasic;
using System.ComponentModel.Design;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Off_brand_Minecraft
{
    public class World
    {
        public int[] worldDimensions = new int[3];
        public bool targetExists;
        public short[,,]? blocks;
        public short[,,]? destructionLevels;
        public bool[,,]? blockPowering;
        public short[,,]? lightLevels;
        public bool[,,]? blocksExposedToSunlight;
        public bool[,,]? blockIsExposed;
        public bool[,,]? blockIsLightUpdated;
        public short[,,] rgbValues = new short[8, 15, 3];
        public bool[,] blockConductivity = { { false, false }, { true, false }, { true, false }, { true, false }, { true, false }, { true, true }, { true, false }, { true, false }, { false, false } };
        short[] destructionSpeeds = { 0, 0, 5, 15, 15, 15, 10, 25 };
        readonly Random r = new();
        public SolidBrush[,] brushes = new SolidBrush[8,15];
        public static float[,] Rref(float[,] Matrix)
        {
            if (Matrix[0,0] == 0) //element (0,0) får inte vara 0, därför flyttas raderna för att sätta en rad där element 1 != 0
            {
                if (Matrix[1, 0] != 0) RowSwap(Matrix, 0, 1);
                else RowSwap(Matrix, 0, 2);
            }
            if (Matrix[0, 0] != 1) RowSimplification(Matrix, 0);  //den översta raden skrivs på enklaste form
            for(int i = 1; i < 3; i++) //den översta raden multiplicerat med ett reellt tal subtraheras från den undre raderna så att alla element i kolumn 0 förutom det i rad 0 blir 0
            {
                if (Matrix[i,0] != 0) RowSubtraction(Matrix, 0, i);
            }
            //därefter upprepas samma process för nästa rad i kolumn 1

            if (Matrix[1, 1] == 0)
            {
                if (Matrix[2, 1] != 0) RowSwap(Matrix, 2, 1); 
            }
            if (Matrix[1, 1] != 1) RowSimplification(Matrix, 1);
            for (int i = 0; i < 3; i++)
            {
                if (i == 1) continue;
                if (Matrix[i, 1] != 0) RowSubtraction(Matrix, 1, i);
            }
            //därefter upprepas samma process för nästa rad i kolumn 2

            if (Matrix[2, 2] != 1) RowSimplification(Matrix, 2);
            for (int i = 0; i < 3; i++)
            {
                if (i == 2) continue;
                if (Matrix[i, 2] != 0) RowSubtraction(Matrix, 2, i);
            }
            //matrisen har nu omvandlats till radkanonisk form och ger därför svaret på ett givet ekvationssystem med tre obekanta
            return Matrix;
        }
        //nedan finns tre sammansatta elementära radoperationer vilka är alla som behövs för att kunna omvandla en matris till radkanonisk form
        //radkanonisk form innebär: Att den första siffran som inte är 0 är 1
        //                          Att alla element under en rads första 1:a är 0
        //                          Att alla element ovanför en rads första 1:a är 0
        //om man sedan ser matrisen som en koefficientmatris och utför matrismultiplikation mellan denna och en variabelmatris kan man se att man löst ett ekvationsystem
        static float[,] RowSwap(float[,] Matrix, int line1, int line2) //metoden byter plats på line1 och line2
        {
            float[] tempRow = { Matrix[line1, 0], Matrix[line1, 1], Matrix[line1, 2], Matrix[line1, 3] };
            for(int i = 0; i < 4; i++)
            {
                Matrix[line1, i] = Matrix[line2, i];
                Matrix[line2, i] = tempRow[i];
            }
            return Matrix;
        }
        static float[,] RowSimplification(float[,] Matrix, int line) //metoden dividerar rad "line" så att första elementet som inte är 0 är 1
        {
            for(int i = 0; i < 3; i++)
            {
                if (Matrix[line, i] != 0)
                {
                    for(int j = 3; j >= 0; j--)
                    {
                        Matrix[line, j] /= Matrix[line, i];
                    }
                    break;
                }
            }
            return Matrix;
        }
        static float[,] RowSubtraction(float[,] Matrix, int line1, int line2) //metoden subtraherar line1 multiplicerat med en reell konstant från line2 så att det första elementet i line2 som inte är 0 blir det
        {
            for(int i = 0; i < 4; i++)
            {
                if(Matrix[line1, i] != 0)
                {
                    for(int j = 3; j >= 0; j--)
                    {
                        Matrix[line2, j] -= Matrix[line1, j] * Matrix[line2, i];
                    }
                    break;
                }
            }
            return Matrix;
        }
        static double DistanceCalculation(Point a, Point b) //omvandla ortogonella avstånd till en vinklad längd enligt avståndsformeln
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        static double CosineRule(double a, double b, double c) //definera en vinkel i en triangel vars tre sidor är definierade med hjälp av cosinussatsen
        {
            return Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));
        }
        public float[,] PerlinNoiseMap(int[] worldSize, int maxGrid, int minGrid)
        {
            //OBS, detta är inte riktigt perlin noise utan endast en approximation av det vilket fungerar på ett fraktalliknande sätt och använder sig av bilinjär interpolation för att få fram hur står del av världens närstående frön som ska tas med
            //riktig perlin noise innehåller vektorer vid varje korsning vars skalärprodukter mot vektorerna till en punkt i rutnätet bestämmer värdet på fröet
            float[,] noiseMap = new float[worldSize[0], worldSize[2]];
            float[,] seed = new float[worldSize[0], worldSize[2]];
            Random seedGenerator = new Random();
            for (int i = 0; i < worldSize[0]; i++)
            {
                for (int j = 0; j < worldSize[2]; j++)
                {
                    seed[i, j] = seedGenerator.Next(128) / 128f;
                }
            }
            for (int i = maxGrid; i > minGrid; i--) //rutor som delar upp varandra i 2x2 steg
            {
                for (int j = 0; j < worldSize[0]; j++)
                {
                    for (int k = 0; k < worldSize[2]; k++)
                    {
                        int interval = (int)Math.Pow(2, i); //längden på en ruta
                        int[] closestSeed = { j - j % interval, k - k % interval }; //avrundning nedåt för att få närmaste nedåtgående rutnätskorsning i båda axlarna
                        int[] distanceFromClosestSeed = { j - closestSeed[0], k - closestSeed[1] }; //avståndet från den valda rutnätskorsningen
                        if (j % interval == 0)
                        {
                            if (k % interval == 0) //om koordinaten är en rutnätskorsning
                            {
                                noiseMap[j, k] += seed[j, k] * interval / (float)Math.Pow(2, maxGrid); //slumpkoordinaten är sig själv
                            }
                            else //om koordinaten enbart korsar med en horisontell linje
                            {
                                noiseMap[j, k] += (seed[closestSeed[0], closestSeed[1]] * (interval - distanceFromClosestSeed[0]) * (interval - distanceFromClosestSeed[1]) +  //ett medelmått mellan övre och nedre rutnätskorsning
                                                    seed[closestSeed[0], closestSeed[1] + interval] * (interval - distanceFromClosestSeed[0]) * distanceFromClosestSeed[1])     //med avseende på avståndet till varje korsning
                                                    / (float)(interval * Math.Pow(2, maxGrid)); //dessa är till för att göra så att de mindre rutorna har en mindre inverkan på resultatet så att slumpandet blir mjukare d.v.s pseudoslumpartat
                            }
                        }
                        else if (k % interval == 0) //om koordinaten enbart korsar med en vertikal linje
                        {
                            noiseMap[j, k] += (seed[closestSeed[0], closestSeed[1]] * (interval - distanceFromClosestSeed[0]) * (interval - distanceFromClosestSeed[1]) +   //ett medelmått mellan vänster och höger rutnätskorsning
                                                seed[closestSeed[0] + interval, closestSeed[1]] * distanceFromClosestSeed[0] * (interval - distanceFromClosestSeed[1]))     //med avseende på avståndet till varje korsning
                                                / (float)(interval * Math.Pow(2, maxGrid));
                        }
                        else //om koordinaten inte korsar någon
                        {
                            noiseMap[j, k] += (seed[closestSeed[0], closestSeed[1]] * (interval - distanceFromClosestSeed[0]) * (interval - distanceFromClosestSeed[1]) +  //ett medelmått mellan övre, nedre, vänster och höger rutnätskorsning
                                                seed[closestSeed[0] + interval, closestSeed[1]] * distanceFromClosestSeed[0] * (interval - distanceFromClosestSeed[1]) +    //med avseende på avståndet till varje korsning
                                                seed[closestSeed[0], closestSeed[1] + interval] * (interval - distanceFromClosestSeed[0]) * distanceFromClosestSeed[1] +
                                                seed[closestSeed[0] + interval, closestSeed[1] + interval] * distanceFromClosestSeed[0] * distanceFromClosestSeed[1])
                                                / (float)(interval * Math.Pow(2, maxGrid));
                        }
                    }
                }
            }
            return noiseMap;
        }
        public void GenerateWorld(ref double[] pos, ref SolidBrush[,] brushes, int[] worldSize, int maxGrid, bool menu)
        {
            worldDimensions = worldSize;
            blocks = new short[worldSize[0], worldSize[1], worldSize[2]]; //en array av alla blockoder för att se vilket block som befinner sig på varje koordinat
            destructionLevels = new short[worldSize[0], worldSize[1], worldSize[2]]; //en array förstörelsenivåer för att lagra hur mycket hälsa varje block har
            blockPowering = new bool[worldSize[0], worldSize[1], worldSize[2]]; //en array som visar om ett block får kraft eller inte
            lightLevels = new short[worldSize[0], worldSize[1], worldSize[2]]; //array för alla blocks ljusnivå
            blocksExposedToSunlight = new bool[worldSize[0], worldSize[1], worldSize[2]];
            blockIsExposed = new bool[worldSize[0], worldSize[1], worldSize[2]];
            blockIsLightUpdated = new bool[worldSize[0], worldSize[1], worldSize[2]];
            rgbValues[2, 14, 0] = 128;
            rgbValues[2, 14, 1] = 128;
            rgbValues[2, 14, 2] = 128; //grå
            rgbValues[3, 14, 0] = 139;
            rgbValues[3, 14, 1] = 69;
            rgbValues[3, 14, 2] = 19; //brun
            rgbValues[4, 14, 0] = 0;
            rgbValues[4, 14, 1] = 160;
            rgbValues[4, 14, 2] = 0; //grön
            rgbValues[5, 14, 0] = 255;
            rgbValues[5, 14, 1] = 255;
            rgbValues[5, 14, 2] = 0; //ljusgrön
            rgbValues[6, 14, 0] = 145;
            rgbValues[6, 14, 1] = 82;
            rgbValues[6, 14, 2] = 45;
            rgbValues[7, 14, 0] = 124;
            rgbValues[7, 14, 1] = 252;
            rgbValues[7, 14, 2] = 0;
            for(int i = 0; i < rgbValues.GetLength(0); i++)
            {
                brushes[i,14] = new SolidBrush(Color.FromArgb(rgbValues[i,14,0], rgbValues[i, 14, 1], rgbValues[i, 14, 2]));
            }
            for (int i = rgbValues.GetLength(1) - 1; i > 0; i--)
            {
                for(int j = 0; j < rgbValues.GetLength(0); j++)
                {
                    for(int k = 0; k < 3; k++)
                    {
                        rgbValues[j,i-1,k] = (short)(rgbValues[j,i,k] * 0.8);
                    }
                    brushes[j,i - 1] = new SolidBrush(Color.FromArgb(rgbValues[j,i - 1,0], rgbValues[j,i - 1,1], rgbValues[j,i - 1,2]));
                }
            }
            /*brushes[1] = new SolidBrush(Color.Black); //färger för de olika blocken
            brushes[2] = new SolidBrush(Color.Gray);
            brushes[3] = new SolidBrush(Color.SaddleBrown);
            brushes[4] = new SolidBrush(Color.Green);
            brushes[5] = new SolidBrush(Color.Yellow);
            brushes[6] = new SolidBrush(Color.FromArgb(145,82,45));
            brushes[7] = new SolidBrush(Color.FromArgb(124, 252, 0));*/
            pos[0] = worldSize[0] * 15;
            pos[2] = worldSize[2] * 15;
            float[,] altitudeMap = PerlinNoiseMap(worldSize,maxGrid, 2); //världens höjd
            float[,] biomeValues = PerlinNoiseMap(worldSize, maxGrid, 4); //världens trädkoncentration
            //Air = 0
            //Bedrock = 1
            //Stone = 2
            //Dirt = 3
            //Grass = 4
            //ClickCube = 5
            //Oak log = 6
            //Oak leaves = 7
            //Door = 8
            for (int i = 0; i < blocks.GetLength(0); i++)
            {
                for (int j = 0; j < blocks.GetLength(1); j++)
                {
                    for (int k = 0; k < blocks.GetLength(2); k++)
                    {
                        destructionLevels[i, j, k] = 100;
                        lightLevels[i, j, k] = 0;
                        if (menu) lightLevels[i, j, k] = 14;
                        if (j < (int)(altitudeMap[i, k] * 100f) + 25)
                        {
                            blocks[i, j, k] = 3;
                        }
                        switch (j) //generera slumpad berggrund
                        {
                            case 0:
                                blocks[i, j, k] = 1;
                                break;
                            case 1:
                                if (r.Next(0, 3) == 1) blocks[i, j, k] = 2;
                                else blocks[i, j, k] = 1;
                                break;
                            case 2:
                                if (r.Next(0, 3) == 1) blocks[i, j, k] = 1;
                                else blocks[i, j, k] = 2;
                                break;
                            case 3:
                                if (r.Next(0, 8) == 1) blocks[i, j, k] = 1;
                                else blocks[i, j, k] = 2;
                                break;
                        }
                        if (j > 0)
                        {
                            if (blocks[i, j - 1, k] == 3 && blocks[i, j, k] == 0)
                            {
                                blocks[i, j, k] = 4;
                                Random placeOak = new Random();
                                if (biomeValues[i,k] > 0.8) //placera ekar utefter noisevärdena av biomen
                                {
                                    if (placeOak.Next((int)(220 / Math.Pow(biomeValues[i, k], 4))) == 1)
                                    {
                                        int[] baseBlock = { i, j, k };
                                        GenerateOak(ref blocks, baseBlock);
                                    }
                                }
                                for (int l = 3; l < j - 24; l++)
                                {
                                    if (blocks[i, j - l, k] == 3) blocks[i, j - l, k] = 2;
                                }
                            }
                        }
                        if (j > 3 && j < 25) blocks[i, j, k] = 2;
                    }
                }
            }
            int heightAtSpawn = 0;
            for(int i = 0; i < blocks.GetLength(1); i++)
            {
                if (blocks[worldSize[0] / 2, i, worldSize[2] / 2] != 0)
                {
                    heightAtSpawn = i * 30;
                }
            }
            pos[1] = heightAtSpawn;
            for(int i = 0; i < blocks.GetLength(0); i++)
            {
                for(int j = 40; j < blocks.GetLength(1); j++)
                {
                    for(int k = 0; k < blocks.GetLength(2); k++)
                    {
                        if(i > 0 && i < blocks.GetLength(0) - 1 && j > 0 && j < blocks.GetLength(1) - 1 && k > 0 && k < blocks.GetLength(2) - 1)
                        {
                            if (blocks[i, j, k] != 0 && (blocks[i + 1, j, k] == 0 || blocks[i - 1, j, k] == 0 || blocks[i, j + 1, k] == 0 || blocks[i, j - 1, k] == 0 || blocks[i, j, k + 1] == 0 || blocks[i, j, k - 1] == 0)) ;
                            {
                                blockIsExposed[i, j, k] = true;
                            }
                        }
                    }
                }
            }
        }
        public void GenerateOak(ref short[,,] world, int[] baseblock)
        {
            Random trunkRandomizer = new Random();
            int trunkHeight = trunkRandomizer.Next(5, 8);
            for(int i = 1; i <= trunkHeight; i++)
            {
                world[baseblock[0], baseblock[1] + i, baseblock[2]] = 6; //generera stammen
                if(trunkHeight - i < 3 && trunkHeight - i > 0)
                {
                    for(int j = -2; j <= 2; j++)
                    {
                        for(int k = -2; k <= 2; k++)
                        {
                            if (j == 0 && k == 0) continue;
                            else
                            {
                                try
                                {
                                    world[baseblock[0] + j, baseblock[1] + i, baseblock[2] + k] = 7; //generera löv
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    for (int k = 0; k <= 1; k++)
                    {
                        if (Math.Abs(i * j) == 1) continue;
                        else if (Math.Abs(i) + Math.Abs(j) + Math.Abs(k) == 0) continue;
                        else
                        {
                            try
                            {
                                world[baseblock[0] + i, baseblock[1] + trunkHeight + k, baseblock[2] + j] = 7; //generera löv
                            }
                            catch { }
                        }
                    }
                }
            }
        }
        public void Draw3DWorld(Graphics g, short[,,] blocks, bool[,,] blockPowering, double[] pos, double[] playerPerspective, Size screen, SolidBrush[,] brushes, int renderDistance, ref int[] targetedSurface, bool goodGraphics, int[] worldSize, bool worldIsMenu)
        {           
            Point screenCenter = new Point();
            screenCenter.X = screen.Width / 2;
            screenCenter.Y = screen.Height / 2;
            targetExists = false;
            int[] playerPos = new int[3];
            bool[] sides = new bool[3]; //[0]: +-X, [1]: +-Y, [2]: +-Z
            for (int i = 0; i < 3; i++) playerPos[i] = (int)pos[i];
            int[] Coords = new int[3]; //blockens koordinater
            int[] order = new int[6];
            int x, y, z; //relativa koordinater för blocket till spelaren
            Point[] targetedPoints = new Point[4];
 
            for (int i = 0; i < renderDistance; i++)
            {
                y = i;
                for (int j = 0; j < renderDistance; j++)
                {
                    for (int k = 0; k < renderDistance; k++)
                    {
                        x = k;
                        z = j;
                        bool[] visibleSides = new bool[6]; //blockens sidor som bestäms som synliga eller ej för att optimera programhastigheten och inte rita upp osynliga ytor
                        //visibleSides 0 = X+
                        //visibleSides 1 = X-
                        //visibleSides 2 = Y+
                        //visibleSides 3 = Y-
                        //visibleSides 4 = Z+
                        //visibleSides 5 = Z-
                        if (x < renderDistance / 2) //Rita upp alla block i formen av en 30 x 30 x 30 kub med spelaren som centrum
                        {
                            Coords[0] = (playerPos[0]) / 30 - renderDistance / 2 + x; //det ritade blocket är: X-
                            sides[0] = false;
                        }
                        else
                        {
                            Coords[0] = (playerPos[0]) / 30 + renderDistance - 1 - x; //det ritade blocket är: X+
                            sides[0] = true;
                        }
                        if (i < renderDistance / 2)
                        {
                            Coords[1] = playerPos[1] / 30 - renderDistance / 2 + i; //det ritade blocket är: Y-
                            sides[1] = false;
                        }
                        else
                        {
                            Coords[1] = playerPos[1] / 30 + renderDistance - 1 - i; //det ritade blocket är: Y+
                            sides[1] = true;
                        }
                        if (z < renderDistance / 2)
                        {
                            Coords[2] = (playerPos[2]) / 30 - renderDistance / 2 + z; //det ritade blocket är: Z-
                            sides[2] = false;
                        }
                        else
                        {
                            Coords[2] = (playerPos[2]) / 30 + renderDistance - 1 - z; //det ritade blocket är: Z+
                            sides[2] = true;
                        }
                                    //när varje sida görs
                        int order0, //upp
                            order1, //ned
                            order2, //fram
                            order3, //bak
                            order4, //vänster
                            order5; //höger
                        if(Math.Abs(x) > Math.Abs(z))
                        {
                            if(Math.Abs(x) > Math.Abs(y)) //ordningen för fasettrendering beroende på varifrån spelaren ser blocket så att ytor bakom andra ytor ej syns
                            {
                                order0 = 2;
                                order1 = 5;
                                order2 = 0;
                                order3 = 3;
                                order4 = 1;
                                order5 = 4;
                            }
                            else
                            {
                                order0 = 0;
                                order1 = 3;
                                order2 = 1;
                                order3 = 4;
                                order4 = 2;
                                order5 = 5;
                            }
                        }
                        else
                        {
                            if (Math.Abs(z) > Math.Abs(y))
                            {
                                order0 = 2;
                                order1 = 5;
                                order2 = 1;
                                order3 = 4;
                                order4 = 0;
                                order5 = 3;
                            }
                            else
                            {
                                order0 = 0;
                                order1 = 3;
                                order2 = 2;
                                order3 = 5;
                                order4 = 1;
                                order5 = 4;
                            }
                        }
                        if (sides[0])
                        {
                            order[order2] = 0;
                            order[order3] = 1;
                        }
                        else
                        {
                            order[order3] = 0;
                            order[order2] = 1;
                        }
                        if (sides[1])
                        {
                            order[order0] = 2;
                            order[order1] = 3;
                        }
                        else
                        {
                            order[order1] = 2;
                            order[order0] = 3;
                        }
                        if (sides[2])
                        {
                            order[order4] = 4;
                            order[order5] = 5;
                        }
                        else
                        {
                            order[order5] = 4;
                            order[order4] = 5;
                        }
                        if (Coords[0] < 0 || Coords[1] < 0 || Coords[2] < 0 ||
                            Coords[0] >= worldSize[0] || Coords[1] >= worldSize[1] || Coords[2] >= worldSize[2]) continue; //om blocket i någon axel befinner sig utanför världen
                        if (blocks[Coords[0], Coords[1], Coords[2]] == 0) continue; //om blocket är luft
                        if (Math.Sqrt(Math.Pow(playerPos[0]/30 - Coords[0], 2) + Math.Pow(playerPos[1]/30 - Coords[1], 2) + Math.Pow(playerPos[2]/30 - Coords[2], 2)) > renderDistance / 2) continue; //om blocket är utanför det renderade klotet
                        if (Coords[0] == worldSize[0] - 1)
                        {
                            visibleSides[0] = true;  //om blocket är i den yttre kanten av världen
                            if(blocks[Coords[0] - 1, Coords[1], Coords[2]] == 0) visibleSides[2] = true;
                        }
                        if (Coords[0] == 0)
                        {
                            visibleSides[1] = true;
                            if (blocks[Coords[0] + 1, Coords[1], Coords[2]] == 0) visibleSides[0] = true;
                        }
                        if (Coords[1] == worldSize[1] - 1)
                        {
                            visibleSides[2] = true;
                            if (blocks[Coords[0], Coords[1] - 1, Coords[2]] == 0) visibleSides[3] = true;

                        } 
                        if (Coords[1] == 0)
                        {
                            visibleSides[3] = true;
                            if (blocks[Coords[0], Coords[1] + 1, Coords[2]] == 0) visibleSides[2] = true;

                        }                
                        if (Coords[2] == worldSize[2] - 1)
                        {
                            visibleSides[4] = true;
                            if (blocks[Coords[0], Coords[1], Coords[2] - 1] == 0) visibleSides[5] = true;
                        }
                        if (Coords[2] == 0)
                        {
                            visibleSides[5] = true;
                            if (blocks[Coords[0], Coords[1], Coords[2] + 1] == 0) visibleSides[4] = true;

                        }
                        if (Coords[0] > 0 && Coords[0] < worldSize[0] - 1)          //om blocket inte befinner sig i x-axelns ändar
                        {                                                                                   //rita ytan om blocket är synligt:
                            if (blocks[Coords[0] + 1, Coords[1], Coords[2]] == 0) visibleSides[0] = true;   //framifrån
                            if (blocks[Coords[0] - 1, Coords[1], Coords[2]] == 0) visibleSides[1] = true;   //bakifrån
                        }
                        if (Coords[1] > 0 && Coords[1] < worldSize[1] - 1)          //om blocket inte befinner sig i y-axelns ändar
                        {                                                                                   //rita ytan om blocket är synligt:
                            if (blocks[Coords[0], Coords[1] + 1, Coords[2]] == 0) visibleSides[2] = true;   //från höger
                            if (blocks[Coords[0], Coords[1] - 1, Coords[2]] == 0) visibleSides[3] = true;   //från vänster
                        }
                        if (Coords[2] > 0 && Coords[2] < worldSize[2] - 1)          //om blocket inte befinner sig i z-axelns ändar
                        {                                                                                   //rita ytan om blocket är synligt:
                            if (blocks[Coords[0], Coords[1], Coords[2] + 1] == 0) visibleSides[4] = true;   //ovanifrån
                            if (blocks[Coords[0], Coords[1], Coords[2] - 1] == 0) visibleSides[5] = true;   //underifrång
                        }
                        if (!blockConductivity[blocks[Coords[0], Coords[1], Coords[2]], 1]) blockPowering[Coords[0], Coords[1], Coords[2]] = false;
                        if (blockPowering[Coords[0], Coords[1], Coords[2]] && blockConductivity[blocks[Coords[0], Coords[1], Coords[2]], 1])
                        {
                            if (blockConductivity[blocks[Coords[0] + 1, Coords[1], Coords[2]], 0] && !blockConductivity[blocks[Coords[0] + 1, Coords[1], Coords[2]], 1]) blockPowering[Coords[0] + 1, Coords[1], Coords[2]] = true;
                            if (blockConductivity[blocks[Coords[0] - 1, Coords[1], Coords[2]], 0] && !blockConductivity[blocks[Coords[0] - 1, Coords[1], Coords[2]], 1]) blockPowering[Coords[0] - 1, Coords[1], Coords[2]] = true;
                            if (blockConductivity[blocks[Coords[0], Coords[1] + 1, Coords[2]], 0] && !blockConductivity[blocks[Coords[0], Coords[1] + 1, Coords[2]], 1]) blockPowering[Coords[0], Coords[1] + 1, Coords[2]] = true;
                            if (blockConductivity[blocks[Coords[0], Coords[1] - 1, Coords[2]], 0] && !blockConductivity[blocks[Coords[0], Coords[1] - 1, Coords[2]], 1]) blockPowering[Coords[0], Coords[1] - 1, Coords[2]] = true;
                            if (blockConductivity[blocks[Coords[0], Coords[1], Coords[2] + 1], 0] && !blockConductivity[blocks[Coords[0], Coords[1], Coords[2] + 1], 1]) blockPowering[Coords[0], Coords[1], Coords[2] + 1] = true;
                            if (blockConductivity[blocks[Coords[0], Coords[1], Coords[2] - 1], 0] && !blockConductivity[blocks[Coords[0], Coords[1], Coords[2] - 1], 1]) blockPowering[Coords[0], Coords[1], Coords[2] - 1] = true;
                        }
                        for (int l = 0; l < visibleSides.Length; l++)
                        {
                            if (visibleSides[order[l]])
                            {
                                int invisibleCorners = 0;
                                int cornersToBeFixed = 0;
                                bool[] cornerIsInvisible = new bool[4];
                                int[,] relativePoints = new int[4, 3];
                                Point[] screenPoints = new Point[4];
                                switch (order[l])
                                {
                                    case 0:
                                        int[] offsets0 = { 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0 }; //Den här delen bestämmer offsets för hörnen på blocket som ska ritas där grundhörnet är X-, Y-, Z- och utefter detta ritas alla 6 sidor
                                        ChooseOffsets(ref relativePoints, offsets0, Coords, playerPos);
                                        break;
                                    case 1:
                                        int[] offsets1 = { 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0 };
                                        ChooseOffsets(ref relativePoints, offsets1, Coords, playerPos);
                                        break;
                                    case 2:
                                        int[] offsets2 = { 0, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 0 };
                                        ChooseOffsets(ref relativePoints, offsets2, Coords, playerPos);
                                        break;
                                    case 3:
                                        int[] offsets3 = { 0, 0, 0, 0, 0, 1, 1, 0, 1, 1, 0, 0 };
                                        ChooseOffsets(ref relativePoints, offsets3, Coords, playerPos);
                                        break;
                                    case 4:
                                        int[] offsets4 = { 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1 };
                                        ChooseOffsets(ref relativePoints, offsets4, Coords, playerPos);
                                        break;
                                    case 5:
                                        int[] offsets5 = { 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0 };
                                        ChooseOffsets(ref relativePoints, offsets5, Coords, playerPos);
                                        break;
                                }
                                if (Math.Sqrt(Math.Pow(relativePoints[0, 0], 2) + Math.Pow(relativePoints[0, 1], 2) + Math.Pow(relativePoints[0, 2], 2)) >= renderDistance * 30)
                                {
                                }
                                float[,] squarePoints = new float[4, 3];
                                float borderWidth = 1;
                                for (int m = 0; m < 4; m++)
                                {
                                    //den här delen skapar ett fullt defifnierat ekvationssystem med tre okända variabler, avståndet mellan kameran och en punkt i världen uttryckt i tre komposanter som är ortogonella i varsin axel av kameran
                                    float xlineXoffset = (float)(Math.Cos(playerPerspective[1]) * Math.Cos(playerPerspective[0]));  //ekvationssystemets matris ser ut som följande:
                                    float xlineYoffset = (float)Math.Sin(playerPerspective[1]);                                     // |dXx  dXy  dXz  dx|
                                    float xlineZoffset = (float)(Math.Cos(playerPerspective[1]) * Math.Sin(playerPerspective[0]));  // |dYx  dYy  dYz  dy|
                                    float ylineXoffset = -(float)(Math.Sin(playerPerspective[1]) * Math.Cos(playerPerspective[0])); // |dZx  dZy  dZz  dz|
                                    float ylineYoffset = (float)Math.Cos(playerPerspective[1]);
                                    float ylineZoffset = -(float)(Math.Sin(playerPerspective[1]) * Math.Sin(playerPerspective[0]));
                                    float zlineXoffset = -(float)(Math.Sin(playerPerspective[0]));
                                    float zlineYoffset = 0;
                                    float zlineZoffset = (float)(Math.Cos(playerPerspective[0]));
                                    float[,] relCoords = new float[3, 4] { { xlineXoffset, ylineXoffset, zlineXoffset, relativePoints[m, 0] }, { xlineYoffset, ylineYoffset, zlineYoffset, relativePoints[m, 1] }, { xlineZoffset, ylineZoffset, zlineZoffset, relativePoints[m, 2] } };
                                    Rref(relCoords);            //den skapade matrisen skrivs om till radkanonisk form genom elementära radoperationer
                                    float X = relCoords[0, 3];  //de ortogonella avstånden i världens koordinatsystem är nu översatta till ortogonella avstånd i kamerans koordinatsystem och kan därför ritas ut mycket lätt
                                    float Y = relCoords[1, 3];
                                    float Z = relCoords[2, 3];
                                    squarePoints[m, 0] = relCoords[0, 3];
                                    squarePoints[m, 1] = relCoords[1, 3];
                                    squarePoints[m, 2] = relCoords[2, 3];
                                    if (X < 0)
                                    {
                                        cornerIsInvisible[m] = true;
                                        invisibleCorners++;
                                        cornersToBeFixed++;
                                    }
                                    if (X != 0)
                                    {
                                        screenPoints[m].X = (int)(screen.Width / 2 + 400 * Z / X); //projicera punkten på skärmen utefter den förflyttade ON-basen
                                        screenPoints[m].Y = (int)(screen.Height / 2 - 400 * Y / X);
                                        if (X < 0.01)
                                        {
                                            screenPoints[m].X = (int)(screen.Width / 2 + 400 * Z / (X + 1));
                                            screenPoints[m].Y = (int)(screen.Height / 2 - 400 * Y / (X + 1));
                                        }
                                    }
                                }
                                if (invisibleCorners <= 3 && invisibleCorners > 0) //om någon/några av sidorna är bakom spelaren
                                {
                                    for (int m = 0; m < 4; m++)
                                    {
                                        if (cornerIsInvisible[m] == true) //programmet nedan gör följande: om punkten är osynlig så förkortas vektorn till närmaste synliga punkt så att den osynliga punkten flyttas till en synlig koordinat
                                        {
                                            int a = 1;
                                            int b = -1;
                                            int p;
                                            if (m == 3) a -= 4;
                                            if (m == 0) b += 4;
                                            if (cornerIsInvisible[m + a] && cornerIsInvisible[m + b]) //om båda närstående hörn är osynliga väljs det motstående
                                            {
                                                if (m > 1) p = -2;
                                                else p = 2; //för att inte gå utanför arrayen
                                            }
                                            else if (cornerIsInvisible[m + a] == false && cornerIsInvisible[m + b] == false) //om båda hörnen syns
                                            {
                                                if (squarePoints[m + a, 0] / Math.Sqrt(Math.Pow(squarePoints[m + a, 1], 2) + Math.Pow(squarePoints[m + a, 2], 2)) > squarePoints[m + b, 0] / Math.Sqrt(Math.Pow(squarePoints[m + b, 1], 2) + Math.Pow(squarePoints[m + b, 2], 2))) p = a; //välj vilket hörn som ger bäst projektion
                                                else p = b;
                                            }
                                            else if (cornerIsInvisible[m + a] == false) p = a;
                                            else if (cornerIsInvisible[m + b] == false) p = b;
                                            else continue;
                                            if (cornerIsInvisible[m + p] == false)
                                            {
                                                float vectorCoef = (squarePoints[m + p, 0]) / (squarePoints[m + p, 0] - squarePoints[m, 0]); //hur mycket vektorn ska kortas för att det osynliga hörnets x-koordinat ska bli 0
                                                float[] vector = new float[3];
                                                for (int o = 0; o < 3; o++)
                                                {
                                                    vector[o] = squarePoints[m + p, o] - squarePoints[m, o]; //vektorn som ska kortas ned
                                                    vector[o] *= vectorCoef; //vektorn kortas ned
                                                    squarePoints[m, o] = squarePoints[m + p, o] - vector[o]; //den osynliga punkten omdefinieras
                                                }
                                                screenPoints[m].X = (int)(screen.Width / 2 + 400 * squarePoints[m, 2]); //skärmpunkterna omdefinieras
                                                screenPoints[m].Y = (int)(screen.Height / 2 - 400 * squarePoints[m, 1]);
                                            }
                                        }
                                    }          
                                }

                                if (invisibleCorners <= 3)
                                {
                                    Pen borderPen = new Pen(Color.Black)
                                    {
                                        Width = borderWidth
                                    };
                                    if (Math.Sqrt(Math.Pow(relativePoints[0,0], 2) + Math.Pow(relativePoints[0, 1], 2) + Math.Pow(relativePoints[0, 2], 2)) <= 120)
                                    {
                                        double totalAngle = 0;
                                        for (int m = 0; m < 4; m++)  //här ritas först linjer mellan varje ritad fyrhörning hörn och hårkorset. Därefter används cosinussatsen för att beräkna vinklarna mellan alla de nyligen framtagna linjerna
                                        {                           //om vinklarna summerar till 360 grader (2 Pi radianer) befinner sig punkten i mitten (hårkorset) inuti den valda polygonen, om vinkelsumman < ett varv så befinner sig punkten utanför polygonen. 
                                            totalAngle += CosineRule(DistanceCalculation(screenPoints[m], screenCenter), DistanceCalculation(screenPoints[(m + 1) % 4], screenCenter), DistanceCalculation(screenPoints[m], screenPoints[(m + 1) % 4]));
                                        }                           //det här antagandet kan ganska lätt bevisas på papper (med det bevisas även att det gäller för alla polygoner)
                                        if (totalAngle >= Math.PI * 2)
                                        {
                                            targetedPoints = screenPoints;  //bestämmer var en röd fyrhörning ska ritas på skärmen
                                            for (int m = 0; m < 3; m++) targetedSurface[m] = Coords[m];        //bestämmer blocket som ska siktas på vid blockinteraktioner
                                            targetedSurface[3] = order[l];
                                            targetExists = true;            //säkertställer så att spelaren inte kan interagera med blocket i världen med koordinaterna [0,0,0]
                                        }
                                    }
                                    if (blocks[Coords[0], Coords[1], Coords[2]] == 5) //om blocket är ett klickblock som ändrar färg beroende på om den är aktiverad eller ej
                                    {
                                        if (blockPowering[Coords[0], Coords[1], Coords[2]])
                                        {
                                            try
                                            {
                                                g.FillPolygon(Brushes.Red, screenPoints);
                                                if (goodGraphics)
                                                    g.DrawPolygon(borderPen, screenPoints);
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                g.FillPolygon(Brushes.Blue, screenPoints);
                                                if (goodGraphics)
                                                    g.DrawPolygon(borderPen, screenPoints);
                                            }
                                            catch { }
                                        }
                                    }
                                    else if (blocks[Coords[0], Coords[1], Coords[2]] == 4) //gräsblock avviker då en sida är grön
                                    {
                                        if (order[l] == 2)
                                        {
                                            try
                                            {
                                                g.FillPolygon(brushes[4,lightLevels[Coords[0], Coords[1], Coords[2]]], screenPoints);
                                                if(goodGraphics) 
                                                    g.DrawPolygon(borderPen, screenPoints);
                                            }
                                            catch { }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                g.FillPolygon(brushes[3, lightLevels[Coords[0], Coords[1], Coords[2]]], screenPoints);
                                                if (goodGraphics) 
                                                    g.DrawPolygon(borderPen, screenPoints);
                                            }
                                            catch { }
                                        }
                                    }
                                    else //de resterande blocken ritas
                                    {
                                        try
                                        {
                                            g.FillPolygon(brushes[blocks[Coords[0], Coords[1], Coords[2]], lightLevels[Coords[0], Coords[1], Coords[2]]], screenPoints);
                                            if (goodGraphics)
                                                g.DrawPolygon(borderPen, screenPoints);
                                        }
                                        catch { }
                                    }
                                }     
                            }
                        }
                    }
                }
            }
            if(targetExists && !worldIsMenu) g.DrawPolygon(Pens.Red, targetedPoints); //markerar den valda ytan på det valda blocket om världen inte är menyvärlden
            /*for (int i = 0; i < brushes.GetLength(0); i++)
            {
                for (int j = 0; j < brushes.GetLength(1); j++)
                {
                    g.FillRectangle(brushes[i, j], i * 160 + j * 10, 0, 8, 10);
                }
            }*/
        }
        private void ChooseOffsets(ref int[,] relativePoints, int[] offsets, int[] Coords, int[] playerPos)
        {
            for (int i = 0; i < relativePoints.GetLength(0); i++)
            {
                for (int j = 0; j < relativePoints.GetLength(1); j++)
                {
                    relativePoints[i, j] = (Coords[j] + offsets[i * 3 + j]) * 30 - playerPos[j]; //utefter offsets så läggs 30 till på blockets hörn då varje block är 30x30x30
                }
            }
        }
        public void DestroyBlock(int[] targetedSurface, ref bool blockDestruction)
        {
            if (targetExists) //förstör det valda blocket
            { 
                destructionLevels[targetedSurface[0], targetedSurface[1], targetedSurface[2]] -= destructionSpeeds[blocks[targetedSurface[0], targetedSurface[1], targetedSurface[2]]];
                if (destructionLevels[targetedSurface[0], targetedSurface[1], targetedSurface[2]] <= 0)
                {
                    for (int i = 1; i <= 14; i++) //block i närheten av förändringen uppdarterar sin ljusstyrka
                    {
                        for (int dx = -i; dx <= i; dx++)
                        {
                            for (int dy = Math.Abs(dx) - i; dy <= i - Math.Abs(dx); dy++)
                            {
                                int dz = i - Math.Abs(dx) - Math.Abs(dy);
                                try
                                {
                                    blockIsLightUpdated[targetedSurface[0] + dx, targetedSurface[1] + dy, targetedSurface[2] + dz] = false;
                                    blockIsLightUpdated[targetedSurface[0] + dx, targetedSurface[1] + dy, targetedSurface[2] - dz] = false;
                                }
                                catch { }
                            }
                        }
                    }
                    blockDestruction = true;
                }
            }
        }
        public void DetermineBlockExposureLevel(ref bool[,,] blocksExposedToSunlight, short[,,] blocks, int x, int z)
        {
            for(int i = blocks.GetLength(1) - 1; i >= 0; i--) //bestämma om ett block är i direkt solljus
            {
                if (blocks[x, i, z] != 0)
                {
                    blocksExposedToSunlight[x, i, z] = true;
                    break;
                }
            }
        }

        public void DetermineLightLevel(ref short[,,] lightLevels, short[,,] blocks, int x, int y, int z, bool[,,] blocksExposedToSunlight)
        {
            if (blocksExposedToSunlight[x, y, z]) //om blocket är direkt under solljus
            {
                bool[,] blockedRays = new bool[3,29];
                blockIsLightUpdated[x, y, z] = true;
                lightLevels[x, y, z] = 14;
                for(int i = 1; i <= 14; i++) //gör en spridning av ljus från en punkt baserad på taxicab-avstånd 
                {
                    for(int dx = -i; dx <= i; dx++)
                    {
                        for(int dy = Math.Abs(dx) - i; dy <= i - Math.Abs(dx); dy++)
                        {                           
                            int dz = i - Math.Abs(dx) - Math.Abs(dy);
                            try
                            {
                                if (lightLevels[x + dx, y + dy, z + dz] < 14 - i && blocks[x + dx, y + dy, z + dz] != 0) //positiv dz
                                {
                                    if (blocksExposedToSunlight[x + dx, y + dy, z + dz] || blockedRays[0, dx + 14] || blockedRays[1, dy + 14] || blockedRays[2, dz + 14] || (blocks[x + dx + 1, y + dy, z + dz] != 0 && blocks[x + dx - 1, y + dy, z + dz] != 0 && blocks[x + dx, y + dy + 1, z + dz] != 0 && blocks[x + dx, y + dy - 1, z + dz] != 0 && blocks[x + dx, y + dy, z + dz + 1] != 0 && blocks[x + dx, y + dy, z + dz - 1] != 0))
                                    {
                                        //undviker block som är utsatta för solljus, block som är skuggade av andra block och block som inte har någon öppen yta
                                    }
                                    else
                                    {
                                        lightLevels[x + dx, y + dy, z + dz] = (short)(14 - i); //ljusstyrkan baseras på avståndet från ljuskällan
                                        blockedRays[0, dx + 14] = true;
                                        blockedRays[1, dy + 14] = true;
                                        blockedRays[2, dz + 14] = true;
                                    }

                                }
                                if (lightLevels[x + dx, y + dy, z - dz] < 14 - i && blocks[x + dx, y + dy, z - dz] != 0)  //negativ dz
                                {
                                    if (blocksExposedToSunlight[x + dx, y + dy, z - dz] || blockedRays[0, dx + 14] || blockedRays[1, dy + 14] || blockedRays[2, 14 - dz] || (blocks[x + dx + 1, y + dy, z - dz] != 0 && blocks[x + dx - 1, y + dy, z - dz] != 0 && blocks[x + dx, y + dy + 1, z - dz] != 0 && blocks[x + dx, y + dy - 1, z - dz] != 0 && blocks[x + dx, y + dy, z - dz + 1] != 0 && blocks[x + dx, y + dy, z - dz - 1] != 0))
                                    {

                                    }
                                    else
                                    {
                                        lightLevels[x + dx, y + dy, z - dz] = (short)(14 - i);
                                        blockedRays[0, dx + 14] = true;
                                        blockedRays[1, dy + 14] = true;
                                        blockedRays[2, 14 - dz] = true;
                                    }
                                }
                            }
                            catch { };
                        }
                    }
                }
            }

        }
    }
}
