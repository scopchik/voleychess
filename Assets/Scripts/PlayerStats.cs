using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Player;

    [SerializeField] private int _startMoney = 10;
    private int _money;

    public int Money => _money;
    public UnityEvent<int> CountOfMoneyChanged;

    private void Awake()
    {
        if (Player == null)
            Player = this;
    }

    private void Start()
    {
        _money = _startMoney;
        CountOfMoneyChanged?.Invoke(_money);
    }

    public void AddMoney(int count)
    {
        _money += count;
        CountOfMoneyChanged?.Invoke(_money);
    }

    public void Dead(int number)
    {
        SceneManager.LoadScene(number);
    }
}