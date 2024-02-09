using Cards;
using Combat;
using Combat.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands
{
    public class Command_InitSide : Pool.Poolable<Command_InitSide>, ICommand {
        public Command_InitSide Init(Actors side, DeckDefinition deck) {
            this.side = side;
            this.deckDef = deck;
            return this;
        }
        Actors side;
        DeckDefinition deckDef;
        public void Execute(Board board)
        {
            board.SetSubCommand(Command_SetDeck.GetAvailable().Init(deckDef.Cards, side));
            board.SetSubCommand(Command_DrawCard.GetAvailable().Init(side, 3));
            Vector2Int pos = new Vector2Int(side == Actors.Actor1 ? 0 : 8, 2);

            board.SetSubCommand(Command_SummonGeneral.GetAvailable().Init(new CardInstance_Unit(deckDef.SideGeneral), pos, side));
            ReturnToPool(this);
        }
    }
    public class Command_SetDeck : Pool.Poolable<Command_SetDeck>, ICommand {
        public Command_SetDeck Init(List<CardDefinition> _cards, Actors _side) {
            cards = new List<CardDefinition>(_cards);
            side = _side;
            return this;
        }
        List<CardDefinition> cards;
        Actors side;


        public void Execute(Board board) {
            foreach (var card in cards) {
                if(card.GetType() == typeof(UnitDefinition)) {
                    board.GetActorReference(side).Deck.Add(new CardInstance_Unit((UnitDefinition)card));
                } else {
                    throw new System.NotImplementedException();
                }
            }

            board.GetActorReference(side).Deck.Shuffle();

            ReturnToPool(this);
        }
    }
    public class Command_AddToDeck : Pool.Poolable<Command_AddToDeck>, ICommand {
        public Command_AddToDeck Init(CardInstance _card, Actors _side){
            cards = new List<CardInstance> { _card };
            side = _side;
            return this;
        }
        public Command_AddToDeck Init(List<CardInstance> _cards, Actors _side)
        {
            cards = new List<CardInstance>(_cards);
            side = _side;
            return this;
        }
        List<CardInstance> cards;
        Actors side;


        public void Execute(Board board)
        {
            board.GetActorReference(side).Deck.AddRange(cards);
            board.GetActorReference(side).Deck.Shuffle();

            ReturnToPool(this);
        }
    }
    public class Command_DrawCard : Pool.Poolable<Command_DrawCard>, ICommand {
        public Command_DrawCard Init(Actors _side) {
            side = _side;
            amount = 1;
            return this;
        }
        public Command_DrawCard Init(Actors _side, int _amount) {
            side = _side;
            amount = _amount;
            return this;
        }

        private Actors side;
        private int amount;

        public void Execute(Board board) {
            for (int i = 0; i < amount; i++) {
                Draw(board);
            }

            ReturnToPool(this);
        }
        private void Draw(Board b) {
            b.GetActorReference(side).Hand[
                b.GetActorReference(side).getFreeHandIndex()
                ] = b.GetActorReference(side).Deck[0];
            b.GetActorReference(side).Deck.RemoveAt(0);

        }
    }
    public class Command_RemoveHandCard : Pool.Poolable<Command_RemoveHandCard>, ICommand {
        public Command_RemoveHandCard Init(int _handIndex, Actors _actor) {
            handIndex = _handIndex;
            actor = _actor;
            return this;
            
        }
        public int handIndex;
        public Actors actor;

        public void Execute(Board board) {
            board.GetActorReference(actor).Hand[handIndex] = null;
            ReturnToPool(this);
        }
    }
    public class Command_ChangeCurrentMana : Pool.Poolable<Command_ChangeCurrentMana>, ICommand {
        public Command_ChangeCurrentMana Init(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;
            return this;
        }
        Actors side;
        int amount;

        public void Execute(Board board)
        {
            board.GetActorReference(side).CurManagems += amount;

            ReturnToPool(this);
        }
    }
    public class Command_ChangeMaxMana : Pool.Poolable<Command_ChangeMaxMana>, ICommand {
        public Command_ChangeMaxMana Init(Actors side, int amount)
        {
            this.side = side;
            this.amount = amount;

            return this;
        }
        Actors side;
        int amount;



        public void Execute(Board board)
        {
            board.GetActorReference(side).MaxManagems += amount;

            ReturnToPool(this);
        }
    }
}