using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace CurvyPath
{
    public class PlayerController : MonoBehaviour
    {
        public static event System.Action PlayerDied;

        public GameObject PlanePrefab;
        public bool isPlay;
        public GameObject originalPoint1;
        public GameObject originalPoint2;
        public int planeCount;
        float offsetShaderX;
        float offsetShaderY;
        bool first;
        bool isCreateBall;
        Vector4 oldOffsetShader;
        int timeCount;
        GameObject playerBall = null;
        public GameObject addScore;
        bool color;
        public GameObject[] ChangeTag;
        public int score;
        string newTag;
        string oldTag;
        int[] array;
        int randomScore;
        public GameObject coin;
        public GameObject addCoin;
        bool spawnMoveBall;
        int ballNumber;
        public ParticleSystem scoreEffect;
        public ParticleSystem collisionEffect;
        GameObject trail;
        float timePast;
        float height;

        private void Awake()
        {
            height = GameManager.Instance.lenght;
            offsetShaderX = -1000;
            offsetShaderY = 819.13f;
            newTag = CharacterManager.Instance.characters[CharacterManager.Instance.CurrentCharacterIndex].tag;
            randomScore = 10;
            array = new int[3];
            array[0] = 0;
            array[1] = 1;
            array[2] = 2;
            oldTag = "ChangeToColor3";
            color = true;
            first = true;
            offsetShaderX = -1000;
            offsetShaderY = 819.13f;
            // Setup
            float distance = Mathf.Abs(originalPoint1.transform.position.x - originalPoint2.transform.position.x);
            if (distance > GameManager.Instance.width)
            {
                originalPoint1.transform.position += new Vector3(Mathf.Abs(distance - GameManager.Instance.width) / 2, 0, 0);
                originalPoint2.transform.position -= new Vector3(Mathf.Abs(distance - GameManager.Instance.width) / 2, 0, 0);
            }
            else
            {
                originalPoint1.transform.position -= new Vector3(Mathf.Abs(distance - GameManager.Instance.width) / 2, 0, 0);
                originalPoint2.transform.position += new Vector3(Mathf.Abs(distance - GameManager.Instance.width) / 2, 0, 0);
            }
            for (int i = 0; i <= GameManager.Instance.limitPlane; i++)
            {
                CreatePlane();
                planeCount += 1;
            }
        }

        void OnEnable()
        {
            GameManager.GameStateChanged += OnGameStateChanged;
        }

        void OnDisable()
        {
            GameManager.GameStateChanged -= OnGameStateChanged;
        }
	
        // Update is called once per frame
        void Update()
        {
            //limitLeft.transform.position = new Vector3(originalPosition.x - GameManager.Instance.width / 2, gameObject.transform.position.y, gameObject.transform.position.z);
            if (isPlay)
            {
                timePast += Time.deltaTime;
                //trail.transform.position = Vector3.SmoothDamp(trail.transform.position, new Vector3(playerBall.transform.position.x, playerBall.transform.position.y, playerBall.transform.position.z), ref velocity, 0.035f / (GameManager.Instance.speed / 200));
            }

            //if (planeCount <= GameManager.Instance.limitPlane)
            //{
            //    CreatePlane();
            //    planeCount += 1;
            //}
            //}
            //pl.transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 5.0f);
        }

        //void LateUpdate()
        //{
        //    if (isPlay)
        //    {
        //        trail.transform.position = Vector3.SmoothDamp(trail.transform.position, new Vector3(playerBall.transform.position.x, playerBall.transform.position.y, playerBall.transform.position.z), ref velocity, 0.035f / (GameManager.Instance.speed / 200));
        //    }
        //}

        public void ReUsePlane(GameObject _planePrefab)
        {
            if (originalPoint1 != null && originalPoint2 != null)
            {
                Vector3 point3;
                Vector3 point4;
                point3 = originalPoint1.transform.position + new Vector3(0, 0, height);
                point4 = originalPoint2.transform.position + new Vector3(0, 0, height);

                _planePrefab.transform.position = (originalPoint1.transform.position - originalPoint2.transform.position) / 2 + originalPoint2.transform.position;

                originalPoint1.transform.position = point3;
                originalPoint2.transform.position = point4;

                if (isCreateBall)
                {
                    if (score >= randomScore)
                    {
                        CreateSlope(_planePrefab.transform.position);
                    }
                    else
                    {
                        if (spawnMoveBall)
                        {
                            CreateMoveBall(_planePrefab.transform.position, ballNumber);
                        }
                        else
                        {
                            int spawnCoinRate = Random.Range(0, 3);
                            CreateBall(_planePrefab.transform.position);
                            if (spawnCoinRate == 1)
                            {
                                CreateCoin(_planePrefab.transform.position);
                            }
                        }
                    }
                    isCreateBall = false;
                }

                _planePrefab.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
                _planePrefab.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));

                if (color)
                {
                    _planePrefab.GetComponent<Renderer>().material.SetVector("_Color", GameManager.Instance.planeFirstColor);
                    color = !color;
                }
                else
                {
                    _planePrefab.GetComponent<Renderer>().material.SetVector("_Color", GameManager.Instance.planeSecondColor);
                    color = !color;
                }

                oldOffsetShader = new Vector4(offsetShaderX, offsetShaderY, 0, 1);
            }
        }

        public void CreatePlane()
        {
            Vector3 point3 = originalPoint1.transform.position + new Vector3(0, 0, height);
            ;
            Vector3 point4 = originalPoint2.transform.position + new Vector3(0, 0, height);

            GameObject _planePrefab = (GameObject)Instantiate(PlanePrefab, (originalPoint1.transform.position - originalPoint2.transform.position) / 2 + originalPoint2.transform.position, Quaternion.Euler(0, 0, 0));

            _planePrefab.GetComponent<CreateMesh>().createMesh(originalPoint1.transform.position, originalPoint2.transform.position, point3, point4);
            originalPoint1.transform.position = point3;
            originalPoint2.transform.position = point4;

            if (isCreateBall)
            {
                if (score >= randomScore)
                {
                    CreateSlope(_planePrefab.transform.position);
                }
                else
                {
                    if (spawnMoveBall)
                    {
                        CreateMoveBall(_planePrefab.transform.position, ballNumber);
                    }
                    else
                    {
                        int spawnCoinRate = Random.Range(0, 3);
                        CreateBall(_planePrefab.transform.position);
                        if (spawnCoinRate == 1)
                        {
                            CreateCoin(_planePrefab.transform.position);
                        }
                    }
                }
                isCreateBall = false;
            }

            _planePrefab.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
            _planePrefab.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));

            if (color)
            {
                _planePrefab.GetComponent<Renderer>().material.SetVector("_Color", GameManager.Instance.planeFirstColor);
                color = !color;
            }
            else
            {
                _planePrefab.GetComponent<Renderer>().material.SetVector("_Color", GameManager.Instance.planeSecondColor);
                color = !color;
            }

            oldOffsetShader = new Vector4(offsetShaderX, offsetShaderY, 0, 1);

            if (first)
            {
                StartCoroutine(instantiate());
                first = false;
            }
        }


        void CreateSlope(Vector3 position)
        {
            int randomTag = Random.Range(0, 2);

            if (ChangeTag[randomTag].tag == oldTag)
            {
                if (randomTag >= 2)
                    randomTag -= 1;
                else
                    randomTag += 1;
            }

            switch (ChangeTag[randomTag].tag)
            {
                case "ChangeToColor1":
                    newTag = "Color1";
                    break;
                case "ChangeToColor2":
                    newTag = "Color2";
                    break;
                case "ChangeToColor3":
                    newTag = "Color3";
                    break;
            }

            GameObject change = (GameObject)Instantiate(ChangeTag[randomTag], position + new Vector3(0, 2.5f, 0), Quaternion.Euler(-44.13f, 0, 0));
            newScale(change, GameManager.Instance.width);
            oldTag = change.tag;
            score = 0;
            change.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
            change.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));
            randomScore = Random.Range(0, 15);
            int spawnMoveBallRate = Random.Range(0, 3);

            if (spawnMoveBallRate == 1)
            {
                spawnMoveBall = true;
                ballNumber = Random.Range(0, 3);
            }
            else
                spawnMoveBall = false;
        }

        void CreateBall(Vector3 position)
        {
            float xRandom = 0;
            string ballTag = "Color3";
            Shuffle(array);
            score += 1;
            for (int i = 0; i < 3; i++)
            {

                switch (i)
                {
                    case 0:
                        xRandom = 0;
                        break;
                    case 1:
                        xRandom = GameManager.Instance.spacing;
                        break;
                    case 2:
                        xRandom = -GameManager.Instance.spacing;
                        break;
                }

                //GameObject _ball = (GameObject)Instantiate(GameManager.Instance.ballLists[0].ball[array[i]], position + new Vector3(xRandom, 2.5f, 10), Quaternion.Euler(-44.13f, 0, 0));

                switch (array[i])
                {
                    case 0:
                        ballTag = "Color3";
                        break;
                    case 1:
                        ballTag = "Color2";
                        break;
                    case 2:
                        ballTag = "Color1";
                        break;
                }

                GameObject _ball = ObjectPooling.SharedInstance.GetPooledObjectByTag(ballTag);
                if (_ball != null)
                {
                    _ball.transform.position = position + new Vector3(xRandom, 2.5f, 10);
                    _ball.transform.rotation = Quaternion.Euler(-44.13f, 0, 0);
                    _ball.SetActive(true);
                }
                _ball.GetComponent<Rigidbody>().isKinematic = true;
                //_ball.GetComponent<MeshFilter>().sharedMesh = playerBall.GetComponent<MeshFilter>().sharedMesh;
                //_ball.transform.localScale = GameManager.Instance.scale;
                _ball.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
                _ball.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));
                //_ball.GetComponent<Renderer>().material.SetFloat("_CamColorDistModifier", 100);

                if (_ball.tag == newTag)
                {
                    _ball.GetComponent<Collider>().isTrigger = true;
                }
            }
        }

        void CreateMoveBall(Vector3 position, int ballNumber)
        {
            int i = Random.Range(1, 3);
            float xRandom = 0;
            string ballTag = "Color3";

            switch (i)
            {
                case 1:
                    xRandom = GameManager.Instance.spacing;
                    break;
                case 2:
                    xRandom = -GameManager.Instance.spacing;
                    break;
            }

            score += 1;
            switch (ballNumber)
            {
                case 0:
                    ballTag = "Color3";
                    break;
                case 1:
                    ballTag = "Color2";
                    break;
                case 2:
                    ballTag = "Color1";
                    break;
            }

            GameObject _moveBall = ObjectPooling.SharedInstance.GetPooledObjectByTag(ballTag);
            if (_moveBall != null)
            {
                _moveBall.transform.position = position + new Vector3(xRandom, 2.5f, 10);
                _moveBall.transform.rotation = Quaternion.Euler(-44.13f, 0, 0);
                _moveBall.SetActive(true);
            }
            _moveBall.GetComponent<Rigidbody>().isKinematic = false;
            //_moveBall.GetComponent<MeshFilter>().sharedMesh = playerBall.GetComponent<MeshFilter>().sharedMesh;
            _moveBall.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
            _moveBall.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));
            //_enemy.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            //_moveBall.GetComponent<Renderer>().material.SetFloat("_CamColorDistModifier", 100);

            switch (i)
            {
                case 1:
                    _moveBall.GetComponent<BallController>().moveRight = false;
                    _moveBall.GetComponent<BallController>().velocity = -0.33f * GameManager.Instance.width;
                    ;
                    break;
                case 2:
                    _moveBall.GetComponent<BallController>().moveRight = true;
                    _moveBall.GetComponent<BallController>().velocity = 0.33f * GameManager.Instance.width;
                    break;
            }

            _moveBall.GetComponent<BallController>().move = true;

            if (_moveBall.tag == newTag)
            {
                _moveBall.GetComponent<Collider>().isTrigger = true;
            }
        }

        void CreateCoin(Vector3 position)
        {
            int i = Random.Range(0, 2);
            float xRandom = 0;

            switch (i)
            {
                case 0:
                    xRandom = 0;
                    break;
                case 1:
                    xRandom = GameManager.Instance.spacing;
                    break;
                case 2:
                    xRandom = -GameManager.Instance.spacing;
                    break;
            }

            GameObject _coin = (GameObject)Instantiate(coin, position + new Vector3(xRandom, 2.5f, 70), Quaternion.Euler(0, 0, 0));
            _coin.GetComponent<Renderer>().material.SetVector("_QOffsetOld", oldOffsetShader);
            _coin.GetComponent<Renderer>().material.SetVector("_QOffset", new Vector4(offsetShaderX, offsetShaderY, 0, 1));
            //_enemy.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            _coin.GetComponent<Renderer>().material.SetFloat("_CamColorDistModifier", 100);
        }

        public void newScale(GameObject theGameObject, float newSize)
        {

            float size = theGameObject.GetComponent<Renderer>().bounds.size.x;

            Vector3 rescale = theGameObject.transform.localScale;

            rescale.x = newSize * rescale.x / size;

            theGameObject.transform.localScale = rescale;

        }

        public void ReScale(GameObject theGameObject, float witdh, float height)
        {

            float sizeX = theGameObject.GetComponent<Renderer>().bounds.size.x;

            Vector3 rescale = theGameObject.transform.localScale;

            rescale.x = witdh * rescale.x / sizeX;

            float sizeZ = theGameObject.GetComponent<Renderer>().bounds.size.z;

            rescale.z = height * rescale.z / sizeZ;

            theGameObject.transform.localScale = rescale;

        }

        IEnumerator instantiate()
        {
            //if(finishLoad)
            yield return new WaitForSeconds(GameManager.Instance.timeCreateBall);
            //else
            //yield return new WaitForSeconds(GameManager.Instance.timeCreateBall*0.1f);

            isCreateBall = true;
            StartCoroutine(instantiate());

        }

        IEnumerator Curved()
        {
            yield return new WaitForSeconds(0.1f);
            float randomx = Random.Range(-1000f, 1000f);
            float randomY = Random.Range(-1000f, 1000f);
            //offsetShaderX = randomx;
            //offsetShaderY = randomY;
            //x = 0;
            //y = 0;
            //Debug.Log("x " + x + " y " + y);
            StartCoroutine(LerpXY(randomx, randomY));

        }

        IEnumerator LerpXY(float randomX, float RandomY)
        {
            var startTime = Time.time;
            float runTime = 20f;
            float timePast = 0;
            float oriX = offsetShaderX;
            float oriY = offsetShaderY;

            while (Time.time < startTime + runTime)
            {
                timePast += Time.deltaTime;
                float factor = timePast / runTime;
                offsetShaderX = Mathf.Lerp(oriX, randomX, factor);
                offsetShaderY = Mathf.Lerp(oriY, RandomY, factor);
                yield return null;
            }
            StartCoroutine(Curved());
        }

        void Shuffle(int[] a)
        {
            // Loops through array
            for (int i = a.Length - 1; i > 0; i--)
            {
                // Randomize a number between 0 and i (so that the range decreases each time)
                int rnd = Random.Range(0, i);

                // Save the value of the current i, otherwise it'll overright when we swap the values
                int temp = a[i];

                // Swap the new and old values
                a[i] = a[rnd];
                a[rnd] = temp;
            }
        }

        // Listens to changes in game state
        void OnGameStateChanged(GameState newState, GameState oldState)
        {
            if (newState == GameState.Playing)
            {
                isPlay = true;
                playerBall = (GameObject)Instantiate(CharacterManager.Instance.characters[CharacterManager.Instance.CurrentCharacterIndex], gameObject.transform.position, Quaternion.Euler(0, CharacterManager.Instance.characters[CharacterManager.Instance.CurrentCharacterIndex].transform.eulerAngles.y, 0));
                //Camera.main.transform.SetParent(playerBall.transform);
                playerBall.GetComponent<PlayerBallController>().scoreEffect = scoreEffect;
                playerBall.GetComponent<PlayerBallController>().collisionEffect = collisionEffect;
                // Do whatever necessary when a new game starts
                playerBall.transform.GetChild(0).gameObject.SetActive(true);
                trail = playerBall.transform.GetChild(0).gameObject;
                trail.transform.SetParent(null);
                StartCoroutine(Curved());
            }
            else
                isPlay = false;
        }

        public void AddScore(Vector3 position, GameObject parent, int score)
        {
            GameObject addScoreCanvas = (GameObject)Instantiate(addScore, position, Quaternion.Euler(Camera.main.transform.eulerAngles.x, 0, 0));
            addScoreCanvas.transform.GetComponentInChildren<Text>().text = "+" + score.ToString();
            addScoreCanvas.transform.SetParent(parent.transform);
            StartCoroutine(MoveAndFade(addScoreCanvas));

        }

        public void AddCoin(Vector3 position, GameObject parent)
        {
            GameObject addCoinCanvas = (GameObject)Instantiate(addCoin, position, Quaternion.Euler(Camera.main.transform.eulerAngles.x, 0, 0));
            addCoinCanvas.transform.SetParent(parent.transform);
            StartCoroutine(MoveAndFade(addCoinCanvas));

        }

        IEnumerator MoveAndFade(GameObject canvas)
        {
            var startTime = Time.time;
            float runTime = 1.5f;
            float timePast = 0;
            var oriPos = canvas.transform.localPosition;
            while (Time.time < startTime + runTime)
            {
                timePast += Time.deltaTime;
                float factor = timePast / runTime;
                if (canvas != null)
                {
                    canvas.transform.localPosition = oriPos + new Vector3(0, factor * 1.0f, 0);
                    canvas.GetComponent<CanvasGroup>().alpha = 1 - factor;
                }
                yield return null;
            }
            if (canvas != null)
            {
                canvas.transform.SetParent(null);
                Destroy(canvas);
            }

        }

        // Calls this when the player dies and game over
        public void Die()
        {
            trail.SetActive(false);
            StopAllCoroutines();
            // Fire event
            if (PlayerDied != null)
                PlayerDied();
        }
    }
}