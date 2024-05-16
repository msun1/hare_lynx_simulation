using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGamesCore.Terrain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungerGames.Interface
{
    public class LynxIntelligenceMichelleSun : LynxIntelligence
    {
        public override Color Color { get { return Color.Purple; } }
        public override string Name { get { return "Michelle's Lynxes"; } }
        public override string BitmapFilename {  get { return "michelle-lynx.png"; } }
        public override string PerceptronFilename => "MichelleSunLynxPerceptron(3)-trainedOffHare(round_5).pcp"; 

        public override double[] GetInputs()
        {
            var nearestHare = GetClosest(lynx: false);

            var secondClosestHare = GetSecondClosestHare();
            var obstacleList = GetObstaclesSorted();

            if (nearestHare != null && secondClosestHare != null && obstacleList.First() != null)
            {
                return new double[] {
                    Stamina,
                    Position.X,
                    Velocity.X,
                    Velocity.Y,
                    Vector2D.Distance(Position, nearestHare.Position),
                    nearestHare.Velocity.X,
                    nearestHare.Velocity.Y,
                    (Position - nearestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, secondClosestHare.Position),
                    Vector2D.Distance(Velocity, secondClosestHare.Velocity),

                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            } 
            else if(nearestHare != null && secondClosestHare == null && obstacleList.First() != null)
            {
                return new double[] {
                    Position.X,
                    Stamina,
                    Velocity.X,
                    Velocity.Y,
                    Position.X + nearestHare.Position.X,
                    nearestHare.Velocity.X,
                    nearestHare.Velocity.Y,
                    (Position - nearestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, nearestHare.Position),
                    Vector2D.Distance(Velocity, nearestHare.Velocity),

                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            }
            else if(nearestHare == null && secondClosestHare != null && obstacleList.First() != null)
            {
                return new double[] {
                    Position.X,
                    Stamina,
                    Velocity.X,
                    Velocity.Y,
                    secondClosestHare.Position.X - Position.X,
                    secondClosestHare.Velocity.X,
                    secondClosestHare.Velocity.Y,
                    (Position - secondClosestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, secondClosestHare.Position),
                    Vector2D.Distance(Velocity, secondClosestHare.Velocity),

                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            }
            else if (nearestHare != null && secondClosestHare != null && obstacleList.First() == null)
            {
                return new double[] {
                    Position.X,
                    Stamina,
                    Velocity.X,
                    Velocity.Y,
                    Vector2D.Distance(Position, nearestHare.Position),
                    nearestHare.Velocity.X,
                    nearestHare.Velocity.Y,
                    (Position - nearestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, secondClosestHare.Position),
                    Vector2D.Distance(Velocity, secondClosestHare.Velocity),
                    
                    Vector2D.Distance(nearestHare.Velocity, Velocity),
                    nearestHare.Velocity.X * Position.Y,
                    secondClosestHare.Velocity.Y - nearestHare.Position.X * Position.Y
                };
            }
            else if (nearestHare != null && secondClosestHare == null && obstacleList.First() == null)
            {
                return new double[] {
                    Position.X,
                    Stamina,
                    Velocity.X,
                    Velocity.Y,
                    Position.X + nearestHare.Position.X,
                    nearestHare.Velocity.X,
                    nearestHare.Velocity.Y,
                    (Position - nearestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, nearestHare.Position),
                    Vector2D.Distance(Velocity, nearestHare.Velocity),
                    Vector2D.Distance(nearestHare.Velocity, Velocity),
                    nearestHare.Velocity.X * Position.Y,
                    nearestHare.Velocity.Y - nearestHare.Position.X * Position.Y
                };
            }
            else if (nearestHare == null && secondClosestHare != null && obstacleList.First() == null)
            {
                return new double[] {
                    Position.X,
                    Stamina,
                    Velocity.X,
                    Velocity.Y,
                    secondClosestHare.Position.X - Position.X,
                    secondClosestHare.Velocity.X,
                    secondClosestHare.Velocity.Y,
                    (Position - secondClosestHare.Position).Azimuthal,
                    Vector2D.Distance(Position, secondClosestHare.Position),
                    Vector2D.Distance(Velocity, secondClosestHare.Velocity),
                    Vector2D.Distance(secondClosestHare.Velocity, Velocity),
                    secondClosestHare.Velocity.X * Position.Y,
                    secondClosestHare.Velocity.Y - secondClosestHare.Position.X * Position.Y
                };
            }
            else
            {
                var corner1 = new Vector2D(0, 0);
                var corner2 = new Vector2D(0, ArenaHeight);
                var corner3 = new Vector2D(ArenaWidth, 0);
                var corner4 = new Vector2D(ArenaWidth, ArenaHeight);

                return new double[] {
                    Position.X,
                    Stamina,
                    Vector2D.Distance(Position, corner1),
                    Vector2D.Distance(Position, corner2),
                    Vector2D.Distance(Position, corner3),
                    Position.X - Velocity.X,
                    Velocity.X,
                    Velocity.Y,
                    Position.X * 2,
                    Vector2D.Distance(Position, corner4),
                    Velocity.X * Position.Y,
                    (Position - Velocity).Azimuthal,
                    Position.Y - Velocity.Y
                };
            }
        }
    }
}
