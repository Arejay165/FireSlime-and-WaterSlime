using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
public class PlayerGUI : MonoBehaviour
{
    public bool isOpened = false;
    public bool isDelaying = false;
    public GameObject menuVote;
    public GameObject pauseButton;
    public GameObject unpauseButton;
    public GameObject pauseVote;
    public GameObject unpauseVote;
    public GameObject restartVote;
    public GameObject quitVote;
    public GameObject voteStatus;

    public void OpenUi()
    {
        if (isDelaying == false)
        {
            CloseAllPanels();
            if (isOpened == false)
            {
                isOpened = true;
                if (GameManager.instance.isPlaying)
                {
                    pauseButton.SetActive(true);
                    unpauseButton.SetActive(false);
                }
                else
                {
                    pauseButton.SetActive(false);
                    unpauseButton.SetActive(true);
                }
                menuVote.SetActive(true);
            }
            else
            {
                isOpened = false;

            }

        }
       
 
    }

    public void CloseAllPanels()
    {
        menuVote.SetActive(false);
        pauseVote.SetActive(false);
        unpauseVote.SetActive(false);
        restartVote.SetActive(false);
        quitVote.SetActive(false);
        voteStatus.SetActive(false);
    }
    public void PauseButtonClicked()
    {
        CloseAllPanels();
        menuVote.SetActive(false);
        GameManager.instance.photonView.RPC("StartVotePause", RpcTarget.OthersBuffered);
        voteStatus.GetComponent<Text>().text = "OTHER PLAYER IS VOTING";
        voteStatus.SetActive(true);

    }
    public void UnpauseButtonClicked()
    {
        CloseAllPanels();
        menuVote.SetActive(false);
        GameManager.instance.photonView.RPC("StartVoteUnpause", RpcTarget.OthersBuffered);
        voteStatus.GetComponent<Text>().text = "OTHER PLAYER IS VOTING";
        voteStatus.SetActive(true);

    }

    public void RestartButtonClicked()
    {
        CloseAllPanels();
        menuVote.SetActive(false);
        GameManager.instance.photonView.RPC("StartVoteRestart", RpcTarget.OthersBuffered);
        voteStatus.GetComponent<Text>().text = "OTHER PLAYER IS VOTING";
        voteStatus.SetActive(true);
    }

    public void QuitButtonClicked()
    {
        CloseAllPanels();
        menuVote.SetActive(false);
        GameManager.instance.photonView.RPC("StartVoteQuit", RpcTarget.OthersBuffered);
        voteStatus.GetComponent<Text>().text = "OTHER PLAYER IS VOTING";
        voteStatus.SetActive(true);
    }

    public void YesButtonClicked()
    {
        voteStatus.GetComponent<Text>().text = "VOTE PASSED";
        voteStatus.SetActive(true);
        StartCoroutine(Delay());
    }

    public void NoButtonClicked()
    {
        voteStatus.GetComponent<Text>().text = "VOTE FAILED";
        voteStatus.SetActive(true);
        StartCoroutine(Delay());
    }

    public void YesPauseButtonClicked()
    {
        GameManager.instance.photonView.RPC("PauseGame", RpcTarget.AllBuffered);
    }

    public void NoPauseButtonClicked()
    {
        GameManager.instance.photonView.RPC("PauseGameFailed", RpcTarget.AllBuffered);
        
      
    }

    public void YesUnpauseButtonClicked()
    {
        GameManager.instance.photonView.RPC("UnpauseGame", RpcTarget.AllBuffered);
    }

    public void NoUnpauseButtonClicked()
    {
        GameManager.instance.photonView.RPC("UnpauseGameFailed", RpcTarget.AllBuffered);


    }

    public void YesRestartButtonClicked()
    {

        GameManager.instance.photonView.RPC("RestartGame", RpcTarget.AllBuffered);

    }

    public void NoRestartButtonClicked()
    {

        GameManager.instance.photonView.RPC("RestartGameFailed", RpcTarget.AllBuffered);

    }

    public void YesQuitButtonClicked()
    {

        GameManager.instance.photonView.RPC("QuitGame", RpcTarget.AllBuffered);

    }

    public void NoQuitButtonClicked()
    {

        GameManager.instance.photonView.RPC("QuitGameFailed", RpcTarget.AllBuffered);

    }
    IEnumerator Delay()
    {
        isDelaying = true;
        yield return new WaitForSeconds(3);
        voteStatus.SetActive(false);
        menuVote.SetActive(false);
        isOpened = false;
        isDelaying = false;
    }
}
