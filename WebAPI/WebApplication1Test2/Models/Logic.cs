using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Chess;

namespace WebApplication1Test2.Models
{
    public class Logic
    {
        private ModelChessDB db;

        public Logic()
        {
            db = new ModelChessDB();
        }
        public Game GetCurrentGame()
        {
            Game game = db
                .Games
                .Where(g => g.Status == "play")
                .OrderBy(g => g.ID)
                .FirstOrDefault();
            if (game == null)
                game = CreateNewGame();
            return game;
        }

        private Game CreateNewGame()
        {
            Game game = new Game();

            Chess.Chess chess = new Chess.Chess();


            game.FEN = chess.fen;
            game.Status = "play";

            db.Games.Add(game);
            db.SaveChanges();

            return game;
        }

        public Game GetGame(int id)
        {
            return db.Games.Find(id);
        }

        internal Game MakeMove(int id, string move)
        {
            Game game = GetGame(id);
            if (game == null) 
                return game;

            if (game.Status != "play") 
                return game;

            Chess.Chess chess = new Chess.Chess(game.FEN);
            Chess.Chess ChessNext = chess.Move(move);

            if (ChessNext.fen == game.FEN)
                return game;

            game.FEN = ChessNext.fen;
            //if (ChessNext.IsCheckmate || chess.IsStalemate)
            //    game.Status = "done";

            db.Entry(game).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return game;
        }
    }
}