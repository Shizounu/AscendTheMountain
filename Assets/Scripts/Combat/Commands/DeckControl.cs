using Cards;
using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{

    public class Command_InitSide : ICommand {
        public Command_InitSide(Actors side, DeckDefinition deck) {
            this.side = side;
            this.deckDef = deck;
        }
        Actors side;
        DeckDefinition deckDef;
        public void Execute(Board board)
        {
            board.SetSubCommand(new Command_SetDeck(deckDef.Cards, side));
            board.SetSubCommand(new Command_DrawCard(side, 3));

            Vector2Int pos = new Vector2Int(side == Actors.Actor1 ? 0 : 8, 2);
            board.SetSubCommand(new Command_SummonUnit(deckDef.SideGeneral, pos, side, true, true));
            
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }


    public class Command_SetDeck : ICommand {
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

    public class Command_AddToDeck : ICommand
    {
        public Command_AddToDeck()
        {

        }
        public Command_AddToDeck(CardDefinition _card, Actors _side)
        {
            cards = new List<CardDefinition> { _card };
            side = _side;
        }
        public Command_AddToDeck(List<CardDefinition> _cards, Actors _side)
        {
            cards = new List<CardDefinition>(_cards);
            side = _side;
        }
        List<CardDefinition> cards;
        Actors side;


        public void Execute(Board board)
        {
            board.getActorReference(side).Deck.AddRange(cards);
            board.getActorReference(side).Deck.Shuffle();
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Command_DrawCard : ICommand {
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

    public class Command_AddCurrentMana : ICommand {
        public Command_AddCurrentMana() { }

        public Command_AddCurrentMana(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;
        }
        private Actors side;
        private int amount;



        public void Execute(Board board)
        {
            board.getActorReference(side).CurManagems += amount;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Command_SubCurrentMana : ICommand
    {
        public Command_SubCurrentMana() { }

        public Command_SubCurrentMana(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;
        }
        private Actors side;
        private int amount;



        public void Execute(Board board)
        {
            board.getActorReference(side).CurManagems -= amount;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }


    public class Command_AddMaxMana : ICommand
    {
        public Command_AddMaxMana() { }

        public Command_AddMaxMana(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;
        }
        private Actors side;
        private int amount;



        public void Execute(Board board)
        {
            board.getActorReference(side).MaxManagems += amount;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
    public class Command_SubMaxMana : ICommand
    {
        public Command_SubMaxMana() { }

        public Command_SubMaxMana(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;
        }
        private Actors side;
        private int amount;



        public void Execute(Board board)
        {
            board.getActorReference(side).MaxManagems -= amount;
        }

        public void Unexecute(Board board)
        {
            throw new System.NotImplementedException();
        }
    }
}
