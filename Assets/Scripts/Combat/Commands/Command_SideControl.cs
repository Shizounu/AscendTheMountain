using Cards;
using Combat;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Commands
{

    class Command_SetDeck : ICommand {
        public Command_SetDeck() {
            
        }
        public Command_SetDeck(List<CardDefinition> _cards, Actors _side) {
            cards = new List<CardDefinition>(_cards);
            side = _side;
        }
        List<CardDefinition> cards;
        Actors side;


        public void Execute(Board board) {
            board.getActorReference(side).Deck = cards;
            board.getActorReference(side).Deck.Shuffle();
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    class Command_DrawCard : ICommand {
        public Command_DrawCard() {
            
        }
        public Command_DrawCard(Actors _side) {
            side = _side;
            amount = 1;
        }
        public Command_DrawCard(Actors _side, int _amount) {
            side = _side;
            amount = _amount;
        }

        private Actors side;
        private int amount;

        public void Execute(Board board) {
            for (int i = 0; i < amount; i++) {
                Draw(board);
            }
        }
        private void Draw(Board b) {
            b.getActorReference(side).Hand[
                b.getActorReference(side).getFreeHandIndex()
                ] = b.getActorReference(side).Deck[0];
            b.getActorReference(side).Deck.RemoveAt(0);

        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_RemoveHandCard : ICommand {
        public Command_RemoveHandCard()
        {

        }
        public Command_RemoveHandCard(int _handIndex, Actors _actor) {
            handIndex = _handIndex;
            actor = _actor;
            
        }
        [SerializeField] private int handIndex;
        [SerializeField] private Actors actor;

        public void Execute(Board board) {
            board.getActorReference(actor).Hand[handIndex] = null;
            // TODO : Make more sophisticated, proper destroy function
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_SummonUnit : ICommand {
        public Command_SummonUnit()
        {
            
        }
        public Command_SummonUnit(Cards.UnitDefinition cardDefinition, Vector2Int _position, Actors _owner) {
            unitDef = cardDefinition;
            position = _position;
            owner = _owner;
        }

        [SerializeField] private Cards.UnitDefinition unitDef;
        [SerializeField] private Vector2Int position;
        [SerializeField] private Actors owner;

        public void Execute(Board board) {
            Unit unit = new Unit(unitDef, owner);
            board.tiles[position.x, position.y].unit = unit;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_RemoveUnit : ICommand {
        public Command_RemoveUnit() {
            
        }
        public Command_RemoveUnit(Unit _unit) {
            unit = _unit;
        }
        [SerializeField] private Unit unit;

        public void Execute(Board board) {
            for (int x = 0; x < board.tiles.GetLength(0); x++) {
                for (int y = 0; y < board.tiles.GetLength(1); y++) {
                    if (board.tiles[x,y].unit == unit) {
                        board.tiles[x, y].unit = null;
                    }

                }
            }
            unit = null;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
