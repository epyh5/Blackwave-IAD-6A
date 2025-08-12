using UnityEngine; 
using UnityEngine.SceneManagement; 
using UnityEngine.UI;  


public class MenuS : MonoBehaviour 
{     
   [Header("Configuración")]     
   public Button botonPlay;     
   public string nombreEscenaJuego = "nivel1";          
   
   [Header("Audio")]     
   public AudioSource audioSource;     
   public AudioClip sonidoClick;      
   
   private void Start()     
   {         
       // Configurar el botón         
       botonPlay.onClick.AddListener(() => {             
           ReproducirSonido();             
           IniciarJuego();         
       });     
   }      
   
   public void IniciarJuego()     
   {         
       SceneManager.LoadScene(nombreEscenaJuego);     
   }      
   
   private void ReproducirSonido()     
   {         
       if (audioSource != null && sonidoClick != null)         
       {             
           audioSource.PlayOneShot(sonidoClick);         
       }     
   } 
}