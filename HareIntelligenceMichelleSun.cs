using Arena;
using DongUtility;
using HungerGames.Animals;
using HungerGames.Interface;
using HungerGamesCore.Terrain;
using System;
using System.Collections.Immutable;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace HungerGames
{
    public class HareIntelligenceMichelleSun : HareIntelligence
    {
        public override Color Color { get { return Color.Purple; } }
        public override string Name { get { return "Michelle's Hares"; } }
        public override string BitmapFilename { get { return "michelle-hare.png"; } }
        public override string PerceptronFilename => "MichelleSunHarePerceptron(3)-trainedOffLynx(round_1).pcp";

        public override double[] GetInputs()
        {
            //get variables
            var nearestLynx = GetClosest(lynx: true);
            var closestHare = GetClosestHare();
            var secondClosestHare = GetSecondClosestHare();
            var obstacleList = GetObstaclesSorted();

            if (closestHare != null && secondClosestHare != null && obstacleList.First() != null && nearestLynx != null)
            {
                return new double[] {
                    //Position.X,
                    //Position.Y,
                    Vector2D.Distance(Position, nearestLynx.Position),
                    Vector2D.Distance(nearestLynx.Velocity, Velocity),
                    Vector2D.Distance(closestHare.Position, Position),
                    Vector2D.Distance(secondClosestHare.Position, Position),
                    Vector2D.Distance(closestHare.Velocity, Velocity),
                    //closestHare.Velocity.X,
                    //closestHare.Velocity.Y,

                    //obstacleList.First().Position.X,
                    //obstacleList.First().Position.Y,
                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            }
            else if (closestHare != null && secondClosestHare == null && obstacleList.First() != null && nearestLynx != null)
            {
                return new double[] {
                    //Position.X,
                    //Position.Y,
                    //Vector2D.Distance(Position, nearestLynx.Position), 
                    //nearestLynx.Position.X, 
                    //nearestLynx.Position.Y,

                    Vector2D.Distance(Position, nearestLynx.Position),
                    Vector2D.Distance(nearestLynx.Velocity, Velocity),
                    Vector2D.Distance(closestHare.Position, Position),
                    nearestLynx.Position.X - Position.X,
                    Vector2D.Distance(closestHare.Velocity, Velocity),

                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            }
            else if (closestHare == null && secondClosestHare != null && obstacleList.First() != null && nearestLynx != null)
            {
                return new double[] {
                    Vector2D.Distance(Position, nearestLynx.Position),
                    Vector2D.Distance(nearestLynx.Velocity, Velocity),
                    nearestLynx.Position.X - Position.X,
                    Vector2D.Distance(secondClosestHare.Position, Position),
                    Vector2D.Distance(secondClosestHare.Velocity, Velocity),

                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Position.X - obstacleList.First().Position.X,
                    Position.Y - obstacleList.First().Position.Y
                };
            }
            else if (nearestLynx != null)
            {
                return new double[] {
                    Vector2D.Distance(Position, nearestLynx.Position),
                    Vector2D.Distance(nearestLynx.Velocity, Velocity),
                    nearestLynx.Position.X - Position.X,
                    (Position - nearestLynx.Position).Azimuthal,
                    nearestLynx.Velocity.Y - Velocity.Y,
                    nearestLynx.Velocity.X - Velocity.X,
                    nearestLynx.Position.Y - Position.Y,
                    nearestLynx.Position.X - Position.X
                };
            }
            else if (obstacleList.First() != null)
            {
                var corner1 = new Vector2D(0, 0);
                var corner2 = new Vector2D(0, ArenaHeight);
                var corner3 = new Vector2D(ArenaWidth, 0);
                var corner4 = new Vector2D(ArenaWidth, ArenaHeight);

                return new double[] {
                    Vector2D.Distance(Position, obstacleList.First().Position),
                    Vector2D.Distance(obstacleList.First().Position, corner1),
                    Vector2D.Distance(obstacleList.First().Position, corner2),
                    Vector2D.Distance(obstacleList.First().Position, corner3),
                    Vector2D.Distance(obstacleList.First().Position, corner4),
                    (Position - obstacleList.First().Position).Azimuthal,
                    Position.X - obstacleList.First().Position.X,
                    Position.Y + obstacleList.First().Position.Y
                };
            }
            else
            {
                var corner1 = new Vector2D(0, 0);
                var corner2 = new Vector2D(0, ArenaHeight);
                var corner3 = new Vector2D(ArenaWidth, 0);
                var corner4 = new Vector2D(ArenaWidth, ArenaHeight);

                return new double[] {
                    //Vector2D.Distance(Position, nearestLynx.Position),
                    //Vector2D.Distance(nearestLynx.Velocity, Velocity),
                    //nearestLynx.Position.X - Position.X,
                    //(Position - nearestLynx.Position).Azimuthal,
                    //nearestLynx.Velocity.Y - Velocity.Y,
                    //nearestLynx.Velocity.X - Velocity.X,
                    //nearestLynx.Position.Y - Position.Y
                    Position.X,
                    Position.Y,
                    Velocity.X,
                    Velocity.Y,
                    Vector2D.Distance(obstacleList.First().Position, corner1),
                    Vector2D.Distance(obstacleList.First().Position, corner2),
                    Vector2D.Distance(obstacleList.First().Position, corner3),
                    Vector2D.Distance(obstacleList.First().Position, corner4),
                };
            }
        }

    }
}
