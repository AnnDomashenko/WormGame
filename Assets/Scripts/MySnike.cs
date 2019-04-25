using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MySnike : MonoBehaviour
{

    public Tilemap Level;
    public Tilemap Items;

    private Vector3 _direction = Vector3.up;
    private float _speed=3f;
    private int Score=0;
    public int quantityFood=3;
    public int record=0;
    private Vector3 _position;
    private Transform _selfTransform;
    private SpriteRenderer _render;
    public GameObject[] _tail = new GameObject[16];
    public GameObject textScore, maxScore, maxScoreLevel;
    public GameObject textSpeed, maxSpeed,maxSpeedLevel;
    public GameObject textWin;
    public GameObject result, resultLevel;
    private Vector3 _oldPosition;
    public Sprite UpSprite, RightSprite, DownSprite, LeftSprite;
    public Sprite TailEndUpSprite, TailEndRightSprite, TailEndDownSprite, TailEndLeftSprite;
    public Sprite HorizontalSprite, VerticalSprite;
    public Sprite TailBendingUpRight, TailBendingUpLeft, TailBendingDownRight, TailBendingDownLeft;
    public TileBase Ground;
    public TileBase Plam;
    public TileBase Pear;
    public TileBase Cherry;
    private TileBase Food;
    private float _currentValue = 150f;
    private float _maxValue;

    private void Start()
    {
        result.SetActive(false);
        resultLevel.SetActive(false);
       // _speed = PlayerPrefs.GetFloat("_speed",3);
       // quantityFood = PlayerPrefs.GetInt("quantityFood",2);
       // Score = PlayerPrefs.GetInt("Score",0);
      //  record = PlayerPrefs.GetInt("record", 0);
        _selfTransform = GetComponent<Transform>();
        _render = GetComponent<SpriteRenderer>();
        _position = _selfTransform.position;
        _maxValue = _currentValue;

        FirstSetFood();


    }

    void Update()
    {
        textSpeed.GetComponent<Text>().text = "Speed: " + (_speed*4 - 11).ToString();
        textScore.GetComponent<Text>().text = "Score: " + Score.ToString();

        TileBase ground = Level.GetTile(Level.WorldToCell(_position - new Vector3(0, 1, 0)));
        if (ground is GrassTile)
        {
            _position += _direction * _speed * Time.deltaTime;
        }
        else
        {
            SetResult();
            TakeDemage(1);
            //PlayerPrefs.DeleteAll();
        }

        Vector3 newPosition = Level.WorldToCell(_position);

        _selfTransform.position = newPosition;

        if (_oldPosition != newPosition)
        {
            TileBase tile = Items.GetTile(Level.WorldToCell(_position - new Vector3(0, 1, 0)));
            if (tile is FootTile)
            {
                var food = (FootTile)tile;
                Score += food.FoodValue;
                textScore.GetComponent<Text>().text = "Score: " + (Score).ToString();

                Items.SetTile(Items.WorldToCell(newPosition - new Vector3(0, 1, 0)), null);
                AddBoneInTail();

            }
            MoveTail(_oldPosition);

        }

        _oldPosition = newPosition;

        if (Score == (record + quantityFood))
        {
            record =Score;
            _speed += 0.25f;
            //quantityFood += 1;
          // PlayerPrefs.SetInt("record", Score);
          // PlayerPrefs.SetInt("Score", Score);
          // PlayerPrefs.SetFloat("_speed",_speed+0.25f);
          // PlayerPrefs.SetInt("quantityFood", quantityFood+2);
             FirstSetFood();
           
        }
    }

    public void FoodSet()
    {
        quantityFood = quantityFood + 5;
        int food = quantityFood;
        Vector3 randomPosition;
        int x;
        int y;
        for (int i = 0; i < food; i++)
        {
            Food = GetFoodTile();
            x = Random.Range(-9, 9);
            y = Random.Range(-6, 6);

            randomPosition = new Vector3(x, y, 0);
            if (Items.GetTile(Items.WorldToCell(randomPosition)) == null)
            {
                Items.SetTile(Items.WorldToCell(randomPosition), Food);
            }
            else { food++; }
        }
    }

    public void FirstSetFood()
    {
       
       // int food = quantityFood+1;
        Vector3 randomPosition;
        int x;
        int y;
        for (int i = 0; i < quantityFood; i++)
        {
            Food = GetFoodTile();
            int rX = Random.Range(-9, 9);
            int rY = Random.Range(-6, 6);

            if ((rX != 0 && rY != -1) || (rX != 0 && rY != -2) || (rX != 0 && rY != -3))
            {
                x = rX;
                y = rY;
            }
            else
            {
                x = Random.Range(1, 9); ;
                y = Random.Range(0, 6); ;
            }
            randomPosition = new Vector3(x, y, 0);
            if (Items.GetTile(Items.WorldToCell(randomPosition)) == null)
            {
                Items.SetTile(Items.WorldToCell(randomPosition), Food);
            }
            else { quantityFood++; }
        }
    }

    public TileBase GetFoodTile()
    {
        int random = Random.Range(0, 3);
        if (random == 0)
        {
            Food = Plam;
        }
        else if (random == 1)
        {
            Food = Pear;
        }
        else
        {
            Food = Cherry;
        }
        return Food;
    }

    public void TakeDemage(float demage)
    {

        _currentValue -= demage;
        _render.color = Color.Lerp(Color.red, Color.white, _currentValue / _maxValue);
        for (int i = 0; i < _tail.Length; i++)
        {
            _tail[i].GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, _currentValue / _maxValue);
        }
        if (_currentValue <= 0)
        {

            Destroy(gameObject);
            for (int i = 0; i < _tail.Length; i++)
            {
                Destroy(_tail[i]);

            }

        }

    }

    public void MoveUp()
    {
        _direction = Vector3.up;
    }

    public void MoveRight()
    {

        _direction = Vector3.right;
    }

    public void MoveDown()
    {

        _direction = Vector3.down;
    }

    public void MoveLeft()
    {
        _direction = Vector3.left;
    }


    //
    //Tail functions 
    //

    public void AddBoneInTail()
    {

        if (_tail.Length == Score + 2) { return; }

        System.Array.Resize(ref _tail, (_tail.Length + 1));
        _tail[_tail.Length - 1] = Instantiate(_tail[_tail.Length - 2]);
        _tail[_tail.Length - 1].GetComponent<SpriteRenderer>().sprite = GetTailEnd(_tail[_tail.Length - 2].transform.position);

    }

    public void MoveTail(Vector3 target)
    {
        for (int i = _tail.Length - 1; i > 0; i--)
        {
            _tail[i].transform.position = _tail[i - 1].transform.position;
        }

        _tail[0].transform.position = target;
        //

        for (int i = _tail.Length - 2; i >= 1; i--)
        {
            var prev = _tail[i + 1];
            var next = _tail[i - 1];
            var current = _tail[i];

            current.GetComponent<SpriteRenderer>().sprite = GetSprite(current.transform.position,
                next.transform.position,
                prev.transform.position);
        }

        _tail[0].GetComponent<SpriteRenderer>().sprite = GetSprite(_tail[0].transform.position,
            _selfTransform.position,
            _tail[1].transform.position);


        _tail[_tail.Length - 1].GetComponent<SpriteRenderer>().sprite = GetTailEnd(_tail[_tail.Length - 2].transform.position);

        _render.sprite = GetTailHead();

    }

    private Sprite GetTailEnd(Vector3 next)
    {
        var direction = (_tail[_tail.Length - 1].transform.position - next).normalized;

        if (direction.x == -1)
        {
            return TailEndLeftSprite;
        }
        else if (direction.x == 1)
        {
            return TailEndRightSprite;
        }
        else if (direction.y == -1)
        {
            return TailEndDownSprite;
        }
        else if (direction.y == 1)
        {
            return TailEndUpSprite;
        }

        return TailEndDownSprite;
    }


    private Sprite GetTailHead()
    {

        if (_direction.x == 1)
        {
            return RightSprite;
        }
        else if (_direction.x == -1)
        {
            return LeftSprite;
        }
        else if (_direction.y == 1)
        {
            return UpSprite;
        }
        else if (_direction.y == -1)
        {
            return DownSprite;
        }

        return null;
    }


    private Sprite GetSprite(Vector3 current, Vector3 next, Vector3 prev)
    {
        var vert = next.x == current.x ? next : prev;
        var hor = next.y == current.y ? next : prev;

        if (prev.y == next.y)
        {
            return HorizontalSprite;
        }
        else if (prev.x == next.x)
        {
            return VerticalSprite;
        }
        else if (prev.x == current.x && current.x == next.x)
        {
            return VerticalSprite;
        }
        else if (vert.y > current.y)
        {
            if (hor.x > current.x)
            {
                return TailBendingUpRight;
            }
            else
            {
                return TailBendingUpLeft;
            }
        }
        else if (vert.y < current.y)
        {
            if (hor.x > current.x)
            {
                return TailBendingDownRight;
            }
            else
            {
                return TailBendingDownLeft;
            }
        }


        return null;
    }


    public void SetResult()
    {
        //result.SetActive(true);
        maxScore.GetComponent<Text>().text = "Your Score: " + (Score).ToString();
        maxSpeed.GetComponent<Text>().text = "Your Speed: " + (_speed * 4 - 11).ToString();
    }

}
