using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Linq;
using static GameBoard.Direction;

namespace Tests
{
    public class GameBoardTestSuite
    {
        private GameBoard gameBoard;
        private readonly List<int> EXPECTEDSTARTSTATE = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 0 };


        [Test]
        public void ShouldSpawnUnshuffled()
        {
            Assert.True(gameBoard.saveState().SequenceEqual(EXPECTEDSTARTSTATE));
        }

        [Test]
        public void ShouldSpawnWithNoCost()
        {
            Assert.Zero(gameBoard.heuristicCost);
        }

        [Test]
        public void ShouldCalculateCorrectHeuristic()
        {
            gameBoard.moveCell(NORTH);
            gameBoard.moveCell(NORTH);
            gameBoard.moveCell(WEST);
            gameBoard.moveCell(WEST);

            Assert.AreEqual(8, gameBoard.heuristicCost);
        }

        [Test]
        public void ShouldNotBeAbleToDoInvalidMove()
        {
            gameBoard.moveCell(EAST);

            Assert.True(gameBoard.saveState().SequenceEqual(EXPECTEDSTARTSTATE));
        }

        [Test]
        public void ShouldMoveUp()
        {
            gameBoard.moveCell(NORTH);
            var expectedState = new List<int> { 1, 2, 3, 4, 5, 0, 7, 8, 6 };
            Assert.True(gameBoard.saveState().SequenceEqual(expectedState));
        }

        [Test]
        public void ShouldMoveLeft()
        {
            gameBoard.moveCell(WEST);
            var expectedState = new List<int> { 1, 2, 3, 4, 5, 6, 7, 0, 8 };
            Assert.True(gameBoard.saveState().SequenceEqual(expectedState));
        }

        [Test]
        public void ShouldShuffle()
        {
            gameBoard.shuffleBoard(150);

            Assert.False(EXPECTEDSTARTSTATE.SequenceEqual(gameBoard.saveState()));
        }

        [Test]
        public void ShouldLoadState()
        {
            var exampleState = new List<int> { 2, 4, 5, 6, 8, 1, 0, 3, 7 };
            gameBoard.loadState(exampleState);

            Assert.True(gameBoard.saveState().SequenceEqual(exampleState));
        }


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
    }
}
