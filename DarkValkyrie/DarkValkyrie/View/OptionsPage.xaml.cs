﻿using DarkValkyrie.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*=============================================================
 * 
 * Adam Coville
 * adam.coville@gmail.com
 * Upsilled ICT_CIV_PROG_201810 
 * Mobile Project
 * 
 * View for the Options Page
 * 
 * Player will be able to control colors, appearance, difficulty
 * and other options here
 * 
 * ==========================================================*/

namespace DarkValkyrie.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class OptionsPage : ContentPage
	{
        internal bool ShowGrid;
        internal bool Positions;

        GamePageViewModel GPVM;
        GamePage CurrentGame;

        //=============================================================

        /*----------------------------------
         * 
         * Constructor
         * 
         * -------------------------------*/

        public OptionsPage(GamePageViewModel gpvm, GamePage current_game)
        {
            InitializeComponent();
            BindingContext = gpvm;

            GPVM = gpvm;
            CurrentGame = current_game;

            ShowGrid = false;
            Positions = false;
        }

        //=============================================================

        /*------------------------------
         * 
         * Turn vertical tracking lines 
         * on or off
         * 
         * ----------------------------*/

        private void Button_Clicked(object sender, EventArgs e)
        {
            //-- if on, turn off

            ShowGrid = GPVM.deviceScreen.ShowGrid;

            if (ShowGrid)
            {
                ShowGrid = false;
            }

            //-- otherwise turn on 

            else
            {
                ShowGrid = true;
            }

            GPVM.deviceScreen.ShowGrid = ShowGrid;
        }

        //================================================================

        /*---------------------------------
         * 
         * Turn on debugging information 
         * about player1 
         * 
         * ------------------------------*/

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Positions = GPVM.Trouble_Visible;

            if (Positions)
            {
                Positions = false;
                CurrentGame.Trouble_Visible = true;
            }
            else
            {
                Positions = true;
                CurrentGame.Trouble_Visible = true;
            }

            GPVM.Trouble_Visible = Positions;
        }
    }
}