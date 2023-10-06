﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Display;

namespace Model
{
    public class Player
    {
        Playground playground = new Playground();
        public int x = 0;
        public int y = Console.WindowHeight - HAUTEURVAISSEAU;
        const int HAUTEURVAISSEAU = 4;

        public Player(int vieVaisseau)                    //Constructor 1
        {

        }

        public void show()                                  //Afficher le vaisseau du joueur
        {
            playground.DisplayShip(x, y);
        }

        public void MovingLeft()
        {
            if (x >= 1)
            {
                --x;
            }
        }

        public void MovingRight()
        {
            if (x <= Console.WindowWidth - 4 - playground.PlayerSprite1.Length)
            {
                ++x;
            }
        }

        public void Shoot()
        {
            Ammo ammo = new Ammo(x, y);
        }
    }
}
