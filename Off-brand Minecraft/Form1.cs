using System.Drawing.Drawing2D;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace Off_brand_Minecraft
{
    /// <summary>
    /// Off-brand Minecraft 1.0
    /// August Andersson TET21 2023-05-20
    /// 
    /// Ett sandboxspel där du kan generera en värld genom ett ungefärligt perlin-noise-system
    /// Spelet kan skapa åtta världar per session men världarna sparas inte vid programnedstängning
    /// Världen består av två lager av världsgenerering, världshöjd och trädkoncentration
    /// Den genererade världen projiceras sedan med ett relativt avancerat 3D-renderingssystem
    /// Spelaren kan hugga och sätta ut block utefter egen fantasi i en 2049 x 256 x 2049 värld
    /// </summary>
    public partial class Form1 : Form
    {
        Button[] worldMenu = new Button[8];
        public Form1()
        {
            InitializeComponent();
            tickSpeed.Start();
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;
            button1.Visible = false; //kontrollbeskrivning
            button1.Enabled = false;
            lbxControls.Visible = false;
            lbxControls.Enabled = false;
            cbxInvertY.Visible = false;
            cbxInvertY.Enabled = false;
            lblPickedBlockAmountIndicator.BackColor = Color.DarkGray;
            for(int i = 0; i < worldMenu.Length; i++) //menyn av världar genereras
            {
                worldMenu[i] = new Button();
                worldMenu[i].AutoSizeMode = AutoSizeMode.GrowAndShrink;
                worldMenu[i].AutoSize = true;
                worldMenu[i].Font = new Font(Font.FontFamily, 12, FontStyle.Bold);
                worldMenu[i].Text = "Generate and enter save file " + (i + 1).ToString();
                worldMenu[i].Tag = i.ToString();
                worldMenu[i].Click += new System.EventHandler(WorldInitiation);
                worldMenu[i].BackColor = btnChange.BackColor;
                worldMenu[i].FlatStyle = btnChange.FlatStyle;
                Controls.Add(worldMenu[i]);
                worldMenu[i].Location = new Point(this.Size.Width / 2 - worldMenu[i].Width / 2, this.Size.Height / 3 + i * 50);
            }
            btnChange.Location = new Point(this.Width / 2 - btnChange.Width / 2, worldMenu[7].Top + 80); //resten av huvudmenyn ordnas
            lbxControls.Location = new Point(this.Width / 2 - lbxControls.Width / 2, worldMenu[0].Top);
            btnControls.Location = new Point(this.Width / 2 - btnChange.Width / 2, worldMenu[7].Top + 80 + btnChange.Height);
            cbxInvertY.Location = new Point(this.Width / 2 + cbxInvertY.Width / 2, worldMenu[0].Top + 50);

        }

        readonly World[] map = { new World() , new World(), new World(), new World(), new World(), new World(), new World(), new World() }; //sessionens världar
        readonly World menu = new World();
        Player player = new Player();
        bool draw;
        bool pause;
        bool gameMenu = false;
        bool mainMenu = false;
        bool drawMenu;
        int renderDistance = 60;
        bool blockDestruction = false;
        bool goodGraphics = true;
        double[,] savedPositions = new double[9, 3];
        short[,,,] savedInventories = new short[8, 10, 3, 2];
        short[,,] savedHotBars = new short[8, 10, 2];
        int[] menuSize = { 129, 256, 129 };

        private void tickSpeed_Tick(object sender, EventArgs e) //varje ms sker detta
        {
            draw = true;
            Invalidate();
            if (mainMenu) //spelaren är i huvudmenyn
            {
                player.playerPerspective[0] += 0.005;
                drawMenu = true;
                Invalidate();
            }
            if (player.playerIsInWorld) //spelaren är i en värld
            {
                if (!pause)
                {
                    lblPickedBlockAmountIndicator.Location = new Point(Cursor.Position.X + 50, Cursor.Position.Y + 50);
                    if (player.pickedUpBlock[1] > 1)
                    {
                        lblPickedBlockAmountIndicator.Text = player.pickedUpBlock[1].ToString();
                        lblPickedBlockAmountIndicator.Visible = true;
                    }
                    else lblPickedBlockAmountIndicator.Visible = false;
                }
                if(pause || player.inventoryIsOpen) //gravitation och fysik sker även om spelaren är i inventariet
                {
                    player.CheckCollisions(ref player.possibleDirections, map[player.worldWithPlayer].blocks, player.playerPos, player.hitBox, ref player.distanceToStop); //räkna alla kollisioner för spelarens alla riktningar
                    if (player.possibleDirections[3]) //om utrymmet under spelaren är fritt
                    {
                        player.verticalspeed -= player.gravity;
                    }
                    else if (!player.possibleDirections[3] && player.verticalspeed < 0) //om spelaren slår i marken
                    {
                        player.verticalspeed = 0;
                        player.playerPos[1] -= player.distanceToStop[3];
                    }
                    if (!player.possibleDirections[2]) player.verticalspeed = 0; //om spelaren slår i taket
                    if (player.possibleDirections[2] || player.possibleDirections[3]) player.playerPos[1] += player.verticalspeed; //om spelaren kan fortsätta sin rörelse
                    else player.verticalspeed = 0; //spelaren kan inte röra sig i någon riktning
                }
                if (pause)
                {
                    if (player.playerIsDestorying) //spelaren siktar på ett block och slår
                    {
                        map[player.worldWithPlayer].DestroyBlock(player.targetedSurface, ref blockDestruction);
                    }
                    if (blockDestruction)
                    {
                        player.PickUpDestroyedBlock(map[player.worldWithPlayer].blocks[player.targetedSurface[0], player.targetedSurface[1], player.targetedSurface[2]]);
                        map[player.worldWithPlayer].blocks[player.targetedSurface[0], player.targetedSurface[1], player.targetedSurface[2]] = 0;
                        player.RefreshHotBarAmountIndicatorValues();
                        blockDestruction = false;

                    }

                    player.walk = true;
                    if (player.controlInputs[0] == player.controlInputs[2] && player.controlInputs[1] == player.controlInputs[3]) player.walk = false; //om alla spelarens motsatta riktningar har samma input, hastigheten annihileras
                    else if (player.controlInputs[0] == player.controlInputs[2]) //om fram och bak har samma input
                    {
                        if (player.controlInputs[1]) player.walkingDirection = 1; //gå vänster eller höger
                        if (player.controlInputs[3]) player.walkingDirection = 3;
                    }
                    else if (player.controlInputs[1] == player.controlInputs[3]) //om vänster och höger har samma input
                    {
                        if (player.controlInputs[0]) player.walkingDirection = 0; //gå fram eller bak
                        if (player.controlInputs[2]) player.walkingDirection = 2;
                    }
                    else if (player.controlInputs[0] && player.controlInputs[1]) player.walkingDirection = 0.5f; //resultantvektorer för olika inputs
                    else if (player.controlInputs[1] && player.controlInputs[2]) player.walkingDirection = 1.5f;
                    else if (player.controlInputs[2] && player.controlInputs[3]) player.walkingDirection = 2.5f;
                    else if (player.controlInputs[3] && player.controlInputs[0]) player.walkingDirection = 3.5f;
                    if (player.walk) player.ChooseDirectionAndWalk(ref player.playerPos, player.possibleDirections, player.playerPerspective[0], player.walkingDirection); //utför rörelse beroende på möjliga riktningar och inputvektor
                    lblPlayerAngle.Text = ((int)player.playerPos[0] / 30).ToString() + "    " + ((int)player.playerPos[1] / 30).ToString() + "    " + ((int)player.playerPos[2] / 30).ToString();
                    if (!player.inventoryIsOpen)
                    {
                        if (Cursor.Position.X >= 1500)
                        {
                            Cursor.Position = new Point(1, Cursor.Position.Y);
                        }
                        if (Cursor.Position.X == 0)
                        {
                            Cursor.Position = new Point(1499, Cursor.Position.Y);
                        }
                        player.playerPerspective[0] = Cursor.Position.X * Math.PI / 750; //översätt pekarens position till perspektivsvinklar
                        if (player.playerPerspective[0] % (Math.PI / 2) == 0)
                        {
                            player.playerPerspective[0] += 0.0001;
                        }
                        if(cbxInvertY.Checked) player.playerPerspective[1] = Cursor.Position.Y * Math.PI / this.Size.Height - Math.PI / 2 + 0.01; //invertera Y
                        else player.playerPerspective[1] = (this.Height - Cursor.Position.Y) * Math.PI / this.Size.Height - Math.PI / 2 + 0.01;
                        if (player.playerPerspective[1] % (Math.PI / 2) == 0)
                        {
                            player.playerPerspective[1] += 0.0001;
                        }
                    }
                }
                draw = true;
                Invalidate();
            }            
        }

        private void WorldInitiation(object sender, EventArgs e)
        {
            lblPlayerAngle.Visible = true;
            btnControls.Visible = false;
            btnControls.Enabled = false;
            for (int i = 0; i < 3; i++)
            {
                savedPositions[8, i] = player.playerPos[i]; //spelaren skickas till senast besökta koordinat
            }
            gameMenu = false;
            for (int i = 0; i < 8; i++) //stäng meny
            {
                worldMenu[i].Visible = false;
                worldMenu[i].Enabled = false;
            }
            btnChange.Visible = false;
            btnChange.Enabled = false;
            pbxLogo.Visible = false;
            mainMenu = false;
            Button btn = (Button)sender;
            int[] worldSize = { 2049, 256, 2049 }; //bestäm världens storlek
            player.worldWithPlayer = (short)int.Parse((string)btn.Tag);
            if (map[player.worldWithPlayer].blocks == null)
            {
                map[player.worldWithPlayer].GenerateWorld(ref player.playerPos, ref map[player.worldWithPlayer].brushes, worldSize, 8); //skapa världen
                player.playerPos[1] += 90;
                btn.Text = "Enter world " + (player.worldWithPlayer + 1); //indikera att en värld existerar på den här platsen
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            player.inventory[j, i, k] = savedInventories[player.worldWithPlayer, j, i, k]; //spelarens inventarie återställs
                        }
                    }
                }
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        player.hotBar[i, j] = savedHotBars[player.worldWithPlayer, i, j]; //spelarens hotbar återställs
                    }
                }
            }
            else
            { //starta en tidigare skapad värld
                for (int i = 0; i < 3; i++)
                {
                    player.playerPos[i] = savedPositions[player.worldWithPlayer, i]; //spelaren skickas till senast besökta koordinat
                    for(int j = 0; j < 10; j++)
                    {
                        for(int k = 0; k < 2; k++)
                        {
                            player.inventory[j, i, k] = savedInventories[player.worldWithPlayer, j, i, k]; //spelarens inventarie återställs
                        }
                    }
                }
                for(int i = 0; i < 10; i++)
                {
                    for(int j = 0; j < 2; j++)
                    {
                        player.hotBar[i,j] = savedHotBars[player.worldWithPlayer, i, j]; //spelarens hotbar återställs
                    }
                }
            }
            player.playerIsInWorld = true;
            pause = true;
            player.OrderInventoryAmountIndicators(this.Size);
            player.OrderHotBarAmountIndicators(this.Size);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Controls.Add(player.inventoryAmountIndicators[i, j]);
                }
                Controls.Add(player.hotBarAmountIndicators[i]);
            }
            player.RefreshHotBarAmountIndicatorValues();
            Cursor.Hide();
        }

        private void KeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
                try
                {
                player.playerPos[1] += 30;
                }
                catch { }
            if (e.KeyCode == Keys.Down)
                try
                {
                player.playerPos[1] -= 30;
                }
                catch { }
            if (e.KeyCode == Keys.Left)
                try 
                { 
                    renderDistance++; 
                }
                catch { }
            if (e.KeyCode == Keys.Right) 
                try 
                { 
                    renderDistance--; 
                }
                catch { }
            if (e.KeyCode == Keys.G)
            {
                goodGraphics = !goodGraphics; //toggla bra grafik
            }
            if(e.KeyCode == Keys.Escape) //stäng spelet eller öppna spelmenyn
            {
                lbxControls.Visible = false;
                lbxControls.Enabled = false;
                cbxInvertY.Visible = false;
                cbxInvertY.Enabled = false;
                player.HideHotBarAmountIndicators();
                if (player.playerIsInWorld)
                {
                    button1.Visible = !button1.Visible;
                    button1.Enabled = button1.Visible;
                    btnControls.Visible = button1.Visible;
                    btnControls.Enabled = button1.Visible;
                    if (button1.Visible)
                    {
                        Cursor.Show();
                    }
                    else Cursor.Hide();
                    pause = !button1.Visible;
                    gameMenu = button1.Visible;
                    if (!button1.Visible) player.RefreshHotBarAmountIndicatorValues();
                }
                else this.Close();
                if (player.inventoryIsOpen && button1.Visible)
                {
                    player.inventoryIsOpen = false;
                    player.RefreshHotBarAmountIndicatorValues();
                    player.RefreshInventoryAmountIndicatorValues();
                    lblPickedBlockAmountIndicator.Visible = false;
                    player.HideInventoryAmountIndicators();
                    Cursor.Position = new Point((int)(750 * player.playerPerspective[0] / Math.PI), (int)((player.playerPerspective[1] - 0.01 + Math.PI / 2) * this.Size.Height / Math.PI));
                    Cursor.Hide();
                    if (player.pickedUpBlock[1] > 0) //lägg ifrån spelaren blocket den höll i om den var i inventariet
                    {
                        bool blockAbsorbed = false;
                        for (int i = 0; i < 10; i++)
                        {
                            if (blockAbsorbed) break;
                            if (player.hotBar[i, 1] == 0)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    player.hotBar[i, k] = player.pickedUpBlock[k];
                                    player.pickedUpBlock[k] = 0;
                                }
                                break;
                            }
                            for (int j = 0; j < 3; j++)
                            {
                                if (player.inventory[i, j, 1] == 0)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        player.inventory[i, j, k] = player.pickedUpBlock[k];
                                        player.pickedUpBlock[k] = 0;
                                    }
                                    blockAbsorbed = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                
            }
            if(e.KeyCode == Keys.W && pause) //framåt
            {
                player.controlInputs[0] = true;
            }
            if (e.KeyCode == Keys.A && pause) //strafe
            {
                player.controlInputs[3] = true;
            }
            if (e.KeyCode == Keys.S && pause) //backa
            {
                player.controlInputs[2] = true;
            }
            if (e.KeyCode == Keys.D && pause) //strafe
            {
                player.controlInputs[1] = true;
            }
            if (e.KeyCode == Keys.Space && !player.possibleDirections[3] && player.verticalspeed == 0 && pause) //hopp
            {
                player.verticalspeed += player.jumpSpeed;
            }
            if (e.KeyCode == Keys.ShiftKey && pause) //sprint
            {
                player.speed = player.sprintSpeed;
            }
            if (e.KeyCode == Keys.D1 && pause) //hotbarruta 0
            {
                player.hotBarTarget = 0;
            }
            if (e.KeyCode == Keys.D2 && pause) //hotbarruta 1
            {
                player.hotBarTarget = 1;
            }
            if (e.KeyCode == Keys.D3 && pause) //hotbarruta 2
            {
                player.hotBarTarget = 2;
            }
            if (e.KeyCode == Keys.D4 && pause) //hotbarruta 3
            {
                player.hotBarTarget = 3;
            }
            if (e.KeyCode == Keys.D5 && pause) //hotbarruta 4
            {
                player.hotBarTarget = 4;
            }
            if (e.KeyCode == Keys.D6 && pause) //hotbarruta 5
            {
                player.hotBarTarget = 5;
            }
            if (e.KeyCode == Keys.D7 && pause) //hotbarruta 6
            {
                player.hotBarTarget = 6;
            }
            if (e.KeyCode == Keys.D8 && pause) //hotbarruta 7
            {
                player.hotBarTarget = 7;
            }
            if (e.KeyCode == Keys.D9 && pause) //hotbarruta 8
            {
                player.hotBarTarget = 8;
            }
            if (e.KeyCode == Keys.D0 && pause) //hotbarruta 9
            {
                player.hotBarTarget = 9;
            }
            if(e.KeyCode == Keys.E && !gameMenu) //öppna inventory
            {
                player.inventoryIsOpen = !player.inventoryIsOpen;
                if (player.inventoryIsOpen)
                {
                    lblPickedBlockAmountIndicator.Visible = true;
                    player.RefreshInventoryAmountIndicatorValues();
                    Cursor.Show();
                    pause = false;
                }
                else
                {
                    if (player.pickedUpBlock[1] > 0) //lägg ifrån spelaren blocket den höll i
                    {
                        bool blockAbsorbed = false;
                        for(int i = 0; i < 10; i++)
                        {
                            if (blockAbsorbed) break;
                            if (player.hotBar[i,1] == 0)
                            {
                                for (int k = 0; k < 2; k++) 
                                {
                                    player.hotBar[i, k] = player.pickedUpBlock[k];
                                    player.pickedUpBlock[k] = 0;
                                }
                                break;
                            }
                            for(int j = 0; j < 3; j++)
                            {
                                if (player.inventory[i, j, 1] == 0)
                                {
                                    for (int k = 0; k < 2; k++)
                                    {
                                        player.inventory[i, j, k] = player.pickedUpBlock[k];
                                        player.pickedUpBlock[k] = 0;
                                    }
                                    blockAbsorbed = true;
                                    break;
                                }
                            }
                        }
                    }
                    lblPickedBlockAmountIndicator.Visible = false;
                    player.HideInventoryAmountIndicators();
                    Cursor.Position = new Point((int)(750 * player.playerPerspective[0] / Math.PI), (int)((player.playerPerspective[1] - 0.01 + Math.PI / 2) * this.Size.Height / Math.PI));
                    Cursor.Hide();
                    pause = true;
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            if (draw && !mainMenu) //rita värld och spelarens gränssnitt
            {

                map[player.worldWithPlayer].Draw3DWorld(e.Graphics, map[player.worldWithPlayer].blocks, player.playerPos, player.playerPerspective, this.Size, map[player.worldWithPlayer].brushes, renderDistance, ref player.targetedSurface, goodGraphics, map[player.worldWithPlayer].worldDimensions);
                e.Graphics.DrawLine(Pens.Black, this.Width / 2, this.Height / 2 - 10, this.Width / 2, this.Height / 2 + 10);
                e.Graphics.DrawLine(Pens.Black, this.Width / 2 - 10, this.Height / 2, this.Width / 2 + 10, this.Height / 2);
                player.DrawGUI(e.Graphics, this.Size, Cursor.Position);
                draw = false;
            }
            if (drawMenu) //rita menybakgrunden
            {
                menu.Draw3DWorld(e.Graphics, menu.blocks, player.playerPos, player.playerPerspective, this.Size, menu.brushes, renderDistance, ref player.targetedSurface, goodGraphics, menu.worldDimensions);
                drawMenu = false;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //avlsuta tidigare rörelser
            if (e.KeyCode == Keys.W)
            {
                player.controlInputs[0] = false; 
            }
            if (e.KeyCode == Keys.A) 
            {
                player.controlInputs[3] = false;
            }
            if (e.KeyCode == Keys.S)
            {
                player.controlInputs[2] = false;
            }
            if (e.KeyCode == Keys.D)
            {
                player.controlInputs[1] = false;
            }
            if (e.KeyCode == Keys.ShiftKey)
            {
                player.speed = player.walkSpeed;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && (!pause || player.inventoryIsOpen) && !button1.Visible && !mainMenu)
            {
                player.PickUpSlot(ref player.pickedUpBlock, Cursor.Position, this.Size); //plocka upp valda block
            }
            if (e.Button == MouseButtons.Right && (!pause || player.inventoryIsOpen) && !button1.Visible && !mainMenu)
            {
                player.PickUpHalf(ref player.pickedUpBlock, Cursor.Position, this.Size); //plocka upp halva innehållet i rutan
            }
            if (e.Button == MouseButtons.Right && pause)
            {
                player.playerIsDestorying = true; //hugg block
                player.RefreshHotBarAmountIndicatorValues();
            }
            if (e.Button == MouseButtons.Left && pause) //sätt ut block
            {
                player.PlayerPlaceBlock(ref map[player.worldWithPlayer].blocks);
                player.RefreshHotBarAmountIndicatorValues();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                player.playerIsDestorying = false; //sluta hugga block
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            lblPlayerAngle.Visible = false; 
            player.HideHotBarAmountIndicators();
            for (int i = 0; i < 3; i++)
            {
                savedPositions[player.worldWithPlayer, i] = player.playerPos[i]; //spelaren sparar den senast besökta koordinaten
                for (int j = 0; j < 10; j++)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        savedInventories[player.worldWithPlayer, j, i, k] = player.inventory[j, i, k]; //inventariet sparas
                    }
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    savedHotBars[player.worldWithPlayer, i, j] = player.hotBar[i, j]; //hotbaren sparas
                }
            }
            player.playerIsInWorld = false; //spelaren går ur världen
            button1.Visible = false;
            button1.Enabled = false;
            for (int i = 0; i < 3; i++) player.playerPos[i] = savedPositions[8, i]; //spelaren går till huvudmenyn
            mainMenu = true;
            for (int i = 0; i < 8; i++) 
            {
                worldMenu[i].Visible = true;
                worldMenu[i].Enabled = true;
            }
            pbxLogo.Visible = true;
            btnChange.Visible = true;
            btnChange.Enabled = true;
            player.playerPerspective[1] = -0.5;

        }

        private void LoadMenu(object sender, EventArgs e) //menyn laddas
        {
            button1.Location = new Point(this.Size.Width / 2 - button1.Width / 2, this.Size.Height / 2 - button1.Height / 2);
            pbxLogo.Location = new Point(this.Size.Width / 2 - pbxLogo.Width / 2, this.Size.Height / 7);
            menu.GenerateWorld(ref player.playerPos, ref menu.brushes, menuSize, 7);
            player.playerPos[1] += 300;
            if (!pause)
            {
                for(int i = 0; i < 3; i++) savedPositions[8,i] = player.playerPos[i];
            }
            mainMenu = true;
            player.playerPerspective[1] = -0.5;
        }

        private void btnChange_Click(object sender, EventArgs e) //bakgrunden byts
        {
            menu.GenerateWorld(ref player.playerPos, ref menu.brushes, menuSize, 7);
            player.playerPos[1] += 300;
        }

        private void btnControls_Click(object sender, EventArgs e) //öppna / stäng kontrollsidan
        {
            lbxControls.Visible = !lbxControls.Visible;
            lbxControls.Enabled = lbxControls.Visible;
            cbxInvertY.Visible = lbxControls.Visible;
            cbxInvertY.Enabled = lbxControls.Visible;
            if (lbxControls.Visible)
            {

                for (int i = 0; i < 8; i++)
                {
                    worldMenu[i].Visible = false;
                    worldMenu[i].Enabled = false;
                }
                btnControls.Text = "Back";
            }
            else
            {
                if (mainMenu)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        worldMenu[i].Visible = true;
                        worldMenu[i].Enabled = true;
                    }
                }
                btnControls.Text = "Controls";
            }
        }
    }
}