using SystemOfExtras;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour, IGameLogic
{
    [SerializeField] private DeckForGame playerDeck, botDeck;
    [SerializeField] private int maxNumberInGame;
    [SerializeField] private TextMeshProUGUI totalNumber, loadsToPlayer, loadsToEnemy;
    private int _stateOfGame;//0: inGame; 1: Win; 2: Lose
    private int _loadPlayer, _loadBot, _totalNumber;
    private bool isTurnOfPlayer;

    private TeaTime _turnOfPlayer, _turnOfBot, _intermediateTurn;

    private void Start()
    {
        _turnOfBot = this.tt().Pause().Add(() =>
        {
            botDeck.BeginTurn();
        }).Loop(handle =>
        {
            if (botDeck.IsFinishTurn())
            {
                handle.Break();
            }
        }).Add(() =>
        {
            isTurnOfPlayer = true;
            botDeck.FinishTurn();
            _intermediateTurn.Play();
        });
        _turnOfPlayer = this.tt().Pause().Add(() =>
        {
            playerDeck.BeginTurn();
        }).Loop(handle =>
        {
            if (playerDeck.IsFinishTurn())
            {
                handle.Break();
            }
        }).Add(() =>
        {
            isTurnOfPlayer = false;
            playerDeck.FinishTurn();
            _intermediateTurn.Play();
        });
        
        _intermediateTurn = this.tt().Pause().Add(() =>
        {
            
        }).Loop(handle =>
        {
            handle.Break();
        }).Add(() =>
        {
            if (isTurnOfPlayer)
            {
                _turnOfPlayer.Play();
            }
            else
            {
                _turnOfBot.Play();
            }
        });

        playerDeck.Configure(this);
        botDeck.Configure(this);
    }

    public int LoadPlayer => _loadPlayer;
    public int LoadEnemy => _loadBot;
    public int StateOfGame => _stateOfGame;

    public void DrawCards()
    {
        botDeck.DrawCards();
        playerDeck.DrawCards();
    }

    public bool DrawCardsIsFinished()
    {
        return playerDeck.DrawIsFinished && botDeck.DrawIsFinished;
    }

    public void StartGame()
    {
        ServiceLocator.Instance.GetService<ILoadScene>().Unlock();
        _intermediateTurn.Play();
    }
    
    public void Win()
    {
        _stateOfGame = 1;
    }
    public void Lose()
    {
        _stateOfGame = 2;
    }

    public void Sum(int cardNumber)
    {
        _totalNumber += cardNumber;
        totalNumber.text = $"Total: {_totalNumber}";
        if (_totalNumber > maxNumberInGame)
        {
            if (isTurnOfPlayer)
            {
                Lose();
            }
            else
            {
                Win();
            }   
        }
    }

    public void AddLoad()
    {
        if (isTurnOfPlayer)
        {
            _loadPlayer++;
            loadsToPlayer.text = $"X {_loadPlayer}";
        }
        else
        {
            _loadBot++;
            loadsToEnemy.text = $"X {_loadBot}";
        }
    }

    public int LoadToPlayer()
    {
        return _loadPlayer;
    }

    public int LoadToEnemy()
    {
        return _loadBot;
    }
}