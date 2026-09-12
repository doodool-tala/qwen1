using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ClashGame.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        
        [Header("Save Settings")]
        public string saveFileName = "savegame.json";
        public string backupFileName = "savegame_backup.json";
        public bool useEncryption = true;
        
        [Header("Paths")]
        private string savePath;
        private string backupPath;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            savePath = Path.Combine(Application.persistentDataPath, saveFileName);
            backupPath = Path.Combine(Application.persistentDataPath, backupFileName);
        }
        
        public void SaveGame(Core.PlayerData playerData)
        {
            try
            {
                // Create save data object
                SaveData saveData = new SaveData
                {
                    playerData = playerData,
                    timestamp = System.DateTime.Now.ToBinary(),
                    version = "1.0.0"
                };
                
                // Serialize to JSON
                string json = JsonUtility.ToJson(saveData, true);
                
                // Encrypt if enabled
                if (useEncryption)
                {
                    json = EncryptString(json);
                }
                
                // Write to file
                File.WriteAllText(savePath, json);
                
                Debug.Log($"Game saved successfully to {savePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save game: {e.Message}");
            }
        }
        
        public Core.PlayerData LoadGame()
        {
            try
            {
                if (!File.Exists(savePath))
                {
                    Debug.Log("No save file found, creating new player data");
                    return CreateNewPlayerData();
                }
                
                // Read from file
                string json = File.ReadAllText(savePath);
                
                // Decrypt if enabled
                if (useEncryption)
                {
                    json = DecryptString(json);
                }
                
                // Deserialize
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                // Validate save data
                if (saveData == null || saveData.playerData == null)
                {
                    Debug.LogWarning("Invalid save data, loading backup");
                    return LoadBackup();
                }
                
                Debug.Log($"Game loaded successfully from {savePath}");
                return saveData.playerData;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load game: {e.Message}, trying backup");
                return LoadBackup();
            }
        }
        
        private Core.PlayerData LoadBackup()
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    Debug.LogWarning("No backup found, creating new player data");
                    return CreateNewPlayerData();
                }
                
                string json = File.ReadAllText(backupPath);
                
                if (useEncryption)
                {
                    json = DecryptString(json);
                }
                
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                if (saveData != null && saveData.playerData != null)
                {
                    Debug.Log("Backup loaded successfully");
                    return saveData.playerData;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load backup: {e.Message}");
            }
            
            return CreateNewPlayerData();
        }
        
        public void CreateBackup(Core.PlayerData playerData)
        {
            try
            {
                SaveData saveData = new SaveData
                {
                    playerData = playerData,
                    timestamp = System.DateTime.Now.ToBinary(),
                    version = "1.0.0"
                };
                
                string json = JsonUtility.ToJson(saveData, true);
                
                if (useEncryption)
                {
                    json = EncryptString(json);
                }
                
                File.WriteAllText(backupPath, json);
                Debug.Log($"Backup created at {backupPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create backup: {e.Message}");
            }
        }
        
        public bool HasSaveFile()
        {
            return File.Exists(savePath);
        }
        
        public void DeleteSave()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                Debug.Log("Save file deleted");
            }
            
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
                Debug.Log("Backup file deleted");
            }
        }
        
        private Core.PlayerData CreateNewPlayerData()
        {
            Core.PlayerData newData = new Core.PlayerData
            {
                gold = 200,
                elixir = 200,
                darkElixir = 0,
                gems = 50,
                trophies = 0,
                experienceLevel = 1,
                experiencePoints = 0
            };
            
            Debug.Log("New player data created");
            return newData;
        }
        
        #region Encryption
        
        private string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes("ClashGameKey12345678901234567890"); // 32 bytes
                aesAlg.IV = Encoding.UTF8.GetBytes("ClashGameIV12345"); // 16 bytes
                
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return System.Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        
        private string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes("ClashGameKey12345678901234567890");
                aesAlg.IV = Encoding.UTF8.GetBytes("ClashGameIV12345");
                
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                using (MemoryStream msDecrypt = new MemoryStream(System.Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        
        #endregion
    }
    
    [System.Serializable]
    public class SaveData
    {
        public Core.PlayerData playerData;
        public long timestamp;
        public string version;
    }
}
