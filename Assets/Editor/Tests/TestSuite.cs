using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;

namespace Tests
{
    public class TestSuite
    {
        private GameBoard gameBoard;

        [SetUp]
        public void Setup()
        {
            GameObject gameBoardGameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/15Game/GameBoard"));
            gameBoard = gameBoardGameObject.GetComponent<GameBoard>();
            gameBoard.createBoard(3);
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(gameBoard.gameObject);
        }

        [UnityTest]
        public IEnumerator ShouldSpawnUnshuffled()
        {
            List<int> expectedState = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 0 };
            List<int> actualState = gameBoard.saveState();

            Assert.True(expectedState.SequenceEqual(actualState));
            yield return null;
        }


    }
}
