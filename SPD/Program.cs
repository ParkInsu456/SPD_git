using System.Numerics;

namespace SPD
{
    public class GameManager
    {
        private Player player;
        private List<Item> inventory;

        private List<Item> storeInventory;
        private List<Dungeon> dungeons;

        private Dictionary<ItemType, int> compareDic;   // 추가요소 장비교체

        private int countClear = 0;
        private int countLevelUp = 1;
        public GameManager()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            player = new Player("Jiwon", "Programmer", 1, 10, 5, 100, 15000);

            inventory = new List<Item>();
            compareDic = new Dictionary<ItemType, int>();   // 추가요소 장비교체

            storeInventory = new List<Item>();
            storeInventory.Add(new Item("무쇠갑옷", "튼튼한 갑옷", ItemType.ARMOR, 0, 5, 0, 500));
            storeInventory.Add(new Item("낡은 검", "낡은 검", ItemType.WEAPON, 2, 0, 0, 1000));
            storeInventory.Add(new Item("골든 헬름", "희귀한 투구", ItemType.ARMOR, 0, 9, 0, 2000));
        
            dungeons = new List<Dungeon>();
            dungeons.Add(new Dungeon("쉬운 던전", 5, 5, 1000));
            dungeons.Add(new Dungeon("보통 던전", 11, 10, 1500));
            dungeons.Add(new Dungeon("어려운 던전", 17, 16, 2000));
        }

        public void StartGame()
        {
            Console.Clear();
            ConsoleUtility.PrintGameHeader();
            MainMenu();
        }

        private void MainMenu()
        {
            // 구성
            // 0. 화면 정리
            Console.Clear();

            // 1. 선택 멘트를 줌
            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.");
            Console.WriteLine("■■■■■■■■■■■■■■■■■■■■■■■■■■■■");
            Console.WriteLine("");

            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 탐색");
            Console.WriteLine("5. 여관");
            Console.WriteLine("");

            // 2. 선택한 결과를 검증함
            int choice = ConsoleUtility.PromptMenuChoice(1, 5);

            // 3. 선택한 결과에 따라 보내줌
            switch (choice)
            {
                case 1:
                    StatusMenu();
                    break;
                case 2:
                    InventoryMenu();
                    break;
                case 3:
                    StoreMenu();
                    break;
                case 4:
                    DungeonMenu();
                    break;
                case 5:
                    InnMenu();
                    break;
            }
            MainMenu();
        }

        private void StatusMenu()
        {
            Console.Clear();

            ConsoleUtility.ShowTitle("■ 상태보기 ■");
            Console.WriteLine("캐릭터의 정보가 표기됩니다.");

            ConsoleUtility.PrintTextHighlights("Lv. ", player.Level.ToString("00"));
            Console.WriteLine("");
            Console.WriteLine($"{player.Name} ( {player.Job} )");

            // TODO : 능력치 강화분을 표현하도록 변경

            player.BonusAtk = inventory.Select(item => item.IsEquipped ? item.Atk : 0).Sum();
            player.BonusDef = inventory.Select(item => item.IsEquipped ? item.Def : 0).Sum();
            player.BonusHp = inventory.Select(item => item.IsEquipped ? item.Hp : 0).Sum();

            ConsoleUtility.PrintTextHighlights("공격력 : ", (player.Atk + player.BonusAtk).ToString(), player.BonusAtk > 0 ? $" (+{player.BonusAtk})" : "");
            ConsoleUtility.PrintTextHighlights("방어력 : ", (player.Def + player.BonusDef).ToString(), player.BonusDef > 0 ? $" (+{player.BonusDef})" : "");
            ConsoleUtility.PrintTextHighlights("체 력 : ", (player.Hp + player.BonusHp).ToString(), player.BonusHp > 0 ? $" (+{player.BonusHp})" : "");

            ConsoleUtility.PrintTextHighlights("Gold : ", player.Gold.ToString());
            Console.WriteLine("");

            Console.WriteLine("0. 뒤로가기");
            Console.WriteLine("");

            switch (ConsoleUtility.PromptMenuChoice(0, 0))
            {
                case 0:
                    MainMenu();
                    break;
            }
        }

        private void InventoryMenu()
        {
            Console.Clear();

            ConsoleUtility.ShowTitle("■ 인벤토리 ■");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");

            for (int i = 0; i < inventory.Count; i++)
            {
                inventory[i].PrintItemStatDescription();
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("1. 장착관리");
            Console.WriteLine("");

            switch (ConsoleUtility.PromptMenuChoice(0, 1))
            {
                case 0:
                    MainMenu();
                    break;
                case 1:
                    EquipMenu();
                    break;
            }
        }

        private void EquipMenu()
        {
            Console.Clear();

            ConsoleUtility.ShowTitle("■ 인벤토리 - 장착 관리 ■");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < inventory.Count; i++)
            {
                inventory[i].PrintItemStatDescription(true, i + 1); // 나가기가 0번 고정, 나머지가 1번부터 배정
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            int KeyInput = ConsoleUtility.PromptMenuChoice(0, inventory.Count);

            switch (KeyInput)
            {
                case 0:
                    InventoryMenu();
                    break;
                default: // 추가요소 장비교체
                    // 같은 아이템 선택하면 장비해제로 가고 같은타입이면 기존장비 해제 후 그 장비 착용
                    // null이면 착용, null이 아니면 키값Type 비교, 같으면 해제 후 착용, 같아도 Value Name이 같으면 장비해제 
                    if (!compareDic.ContainsKey(inventory[KeyInput - 1].Type))
                    {
                    inventory[KeyInput - 1].ToggleEquipStatus();
                        compareDic.Add(inventory[KeyInput - 1].Type, KeyInput - 1);
                    }
                    else // 같은 자리에 장비를 끼고있다 == 선택된 장비와 타입이 같다 같은 타입이면서 다른 장비이면 바꿔끼기.
                    {
                        foreach (KeyValuePair<ItemType, int> dic in compareDic)  
                        {
                            if(!(dic.Value == KeyInput - 1)) //dic에 저장된 장비가 선택한 장비와 같은 장비인지 비교 다르면
                            {
                                inventory[dic.Value].ToggleEquipStatus(); // 기존장비 해제
                                compareDic.Remove(dic.Key); // 기존장비 삭제     
                                
                                inventory[KeyInput - 1].ToggleEquipStatus(); //골랐던 장비 착용
                                compareDic.Add(inventory[KeyInput - 1].Type, KeyInput - 1);
                                break;
                            }
                            //같은 타입이면서 같은 장비이면 장비해제.
                            else 
                            {
                                inventory[KeyInput - 1].ToggleEquipStatus();
                                compareDic.Remove(dic.Key); // 기존장비 삭제 
                                break; 
                            }
                        }
                    }        

                    EquipMenu();
                    break;
            }
        }

        private void StoreMenu()
        {
            Console.Clear();

            ConsoleUtility.ShowTitle("■ 상점 ■");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");
            Console.WriteLine("[보유 골드]");
            ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < storeInventory.Count; i++)
            {
                storeInventory[i].PrintStoreItemDescription();
            }
            Console.WriteLine("");
            Console.WriteLine("1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("");
            switch (ConsoleUtility.PromptMenuChoice(0, 2))
            {
                case 0:
                    MainMenu();
                    break;
                case 1:
                    PurchaseMenu();
                    break;
                case 2:
                    SellMenu();
                    break;
            }
        }

        private void PurchaseMenu(string? prompt = null)
        {
            if (prompt != null)
            {
                // 1초간 메시지를 띄운 다음에 다시 진행
                Console.Clear();
                ConsoleUtility.ShowTitle(prompt);
                Thread.Sleep(1000);
            }

            Console.Clear();

            ConsoleUtility.ShowTitle("■ 상점 ■");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");
            Console.WriteLine("[보유 골드]");
            ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < storeInventory.Count; i++)
            {
                storeInventory[i].PrintStoreItemDescription(true, i + 1);
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("");

            int keyInput = ConsoleUtility.PromptMenuChoice(0, storeInventory.Count);

            switch (keyInput)
            {
                case 0:
                    StoreMenu();
                    break;
                default:
                    // 1 : 이미 구매한 경우
                    if (storeInventory[keyInput - 1].IsPurchased) // index 맞추기
                    {
                        PurchaseMenu("이미 구매한 아이템입니다.");
                    }
                    // 2 : 돈이 충분해서 살 수 있는 경우
                    else if (player.Gold >= storeInventory[keyInput - 1].Price)
                    {
                        player.Gold -= storeInventory[keyInput - 1].Price;
                        storeInventory[keyInput - 1].Purchase();
                        inventory.Add(storeInventory[keyInput - 1]);
                        PurchaseMenu();
                    }
                    // 3 : 돈이 모자라는 경우
                    else
                    {
                        PurchaseMenu("Gold가 부족합니다.");
                    }
                    break;
            }
        }

        private void SellMenu(string? prompt = null) // 추가요소 상점 판매
        {
            if (prompt != null)
            {
                // 1초간 메시지를 띄운 다음에 다시 진행
                Console.Clear();
                ConsoleUtility.ShowTitle(prompt);
                Thread.Sleep(1000);
            }
            Console.Clear();

            ConsoleUtility.ShowTitle("■ 상점 ■");
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.");
            Console.WriteLine("");
            Console.WriteLine("[보유 골드]");
            ConsoleUtility.PrintTextHighlights("", player.Gold.ToString(), " G");
            Console.WriteLine("");
            Console.WriteLine("[아이템 목록]");
            for (int i = 0; i < inventory.Count; i++)
            {
                inventory[i].PrintItemStatDescription(true, i + 1);
            }
            Console.WriteLine("");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("");

            int keyInput = ConsoleUtility.PromptMenuChoice(0, inventory.Count);

            void Sell(int keyInput)
            {
                player.Gold += (int)(inventory[keyInput - 1].Price * 0.85f);
                foreach (var item in storeInventory)
                {
                    if (item.Name == inventory[keyInput - 1].Name)
                    {
                        item.Purchase(); // 불값 되돌림
                        break;
                    }
                }
                inventory.Remove(inventory[keyInput - 1]);
            }

            switch (keyInput)
            {
                case 0:
                    StoreMenu();
                    break;
                default:
                    // 1 : 장비한 아이템인 경우
                    if (inventory[keyInput - 1].IsEquipped) // index 맞추기
                    {
                        Console.WriteLine("정말로 장비한 아이템을 파시겠습니까?");
                        Console.WriteLine("0. 아니오     1. 예");
                        int keyInput2 = ConsoleUtility.PromptMenuChoice(0, 1);
                        switch (keyInput2)
                        {
                            case 0:
                                SellMenu();
                                break;
                            case 1:
                                // 장비해제
                                inventory[keyInput - 1].ToggleEquipStatus();
                                // 판매
                                Sell(keyInput);
                                break;
                        }
                    }
                    // 2 : 장비하지 않은 아이템인 경우
                    else
                    {
                        Sell(keyInput);
                        SellMenu();
                    }
                    break;
            }
        }

        public void DungeonMenu(string? prompt = null)
        {
            if (prompt != null)
            {
                // 1초간 메시지를 띄운 다음에 다시 진행
                Console.Clear();
                ConsoleUtility.ShowTitle(prompt);
                Console.Write("체력 ");
                Console.ForegroundColor = ConsoleColor.DarkRed;                
                Console.WriteLine(player.Hp.ToString() + " -> " + (player.Hp -(int)(player.Hp*0.5)).ToString());
                //HP 적용
                player.Hp -= ((int)(player.Hp * 0.5));
                Console.ResetColor();
                Thread.Sleep(1000);
            }

            Console.Clear();

            ConsoleUtility.ShowTitle("■ 던전탐색 ■");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("");           
           
            Console.WriteLine("");
            Console.WriteLine("1. 쉬운 던전");
            Console.WriteLine("2. 일반 던전");
            Console.WriteLine("3. 어려운 던전");
            Console.WriteLine("0. 나가기");
            Console.WriteLine("");

            int keyInput = ConsoleUtility.PromptMenuChoice(0, dungeons.Count);
            Random random = new Random();

            switch (keyInput)
            {
                case 0:
                    MainMenu();
                    break;
                case 1:
                    if (dungeons[keyInput-1].NeedDef > player.Def)
                    {
                        if(4 > random.Next(0,10))
                        {
                            DungeonMenu("던전의 강적을 만나 철수했습니다.");
                        }
                        else DungeonClear(keyInput);
                    }
                    else DungeonClear(keyInput);
                    break;
                case 2:
                    if (dungeons[keyInput - 1].NeedDef > player.Def)
                    {
                        if (4 > random.Next(0, 10))
                        {
                            DungeonMenu("던전의 강적을 만나 철수했습니다.");
                        }
                        else DungeonClear(keyInput);
                    }
                    else DungeonClear(keyInput);
                    break;
                case 3:
                    if (dungeons[keyInput - 1].NeedDef > player.Def)
                    {
                        if (4 > random.Next(0, 10))
                        {
                            DungeonMenu("던전의 강적을 만나 철수했습니다.");
                        }
                        else DungeonClear(keyInput);
                    }
                    else DungeonClear(keyInput);
                    break;

            }
        }
        public void DungeonClear(int i)
        {
            Console.Clear();
              
            ConsoleUtility.ShowTitle("■ 던전 클리어 ■");
            Console.WriteLine($"이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            Console.WriteLine("");
            Console.WriteLine("[탐험 결과]");
            Console.WriteLine("");
            Random rnd = new Random();
            Console.Write("체력 ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            int consumeHp = rnd.Next(20, 36) - (dungeons[i - 1].NeedDef - (player.Def + player.BonusDef));            
            Console.WriteLine(player.Hp.ToString() + " -> " + (player.Hp - consumeHp).ToString());
            player.Hp -= consumeHp;            
            Console.ResetColor();
            Console.Write("Gold ");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            int rewardGold = (int)(dungeons[i - 1].Reward+ dungeons[i - 1].Reward * rnd.Next(player.Atk, player.Atk * 2) * 0.01);            
            Console.WriteLine(player.Gold.ToString() + " G -> " + (player.Gold + rewardGold).ToString() + " G");
            player.Gold += rewardGold;
            Console.ResetColor();
            countClear++;
            if (countClear == countLevelUp)
            {
                Console.WriteLine("");
                Console.WriteLine("레벨업!");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(player.Level.ToString() + " -> " + (player.Level+1).ToString() );
                Console.ResetColor();
                player.LevelUp();
                countLevelUp++;
                countClear = 0;
            }

            Console.WriteLine("");
            Console.WriteLine("0. 나가기");

            switch (ConsoleUtility.PromptMenuChoice(0, 0))
            {
                case 0:
                    DungeonMenu();
                    break;               
            }
        }

        public void InnMenu(string? prompt = null)
        {
            if (prompt != null)
            {
                // 1초간 메시지를 띄운 다음에 다시 진행
                Console.Clear();
                ConsoleUtility.ShowTitle(prompt);
                Thread.Sleep(1000);
            }

            Console.Clear();

            ConsoleUtility.ShowTitle("■ 여관 ■");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("500");
            Console.ResetColor();            
            Console.Write(" G 를 내면 체력을 회복할 수 있습니다.");
            ConsoleUtility.PrintTextHighlights(" (보유 골드 : ", player.Gold.ToString(), " G)");
            ConsoleUtility.PrintTextHighlights("현재 체력 ", player.Hp.ToString());

            Console.WriteLine("1. 휴식하기");        
            Console.WriteLine("0. 나가기");
            Console.WriteLine("");

            switch (ConsoleUtility.PromptMenuChoice(0, 1))
            {
                case 0:
                    DungeonMenu();
                    break;
                case 1:
                    if (player.Gold >= 500)
                    {
                        player.Gold -= 500;
                        // 체력회복
                        if (!(player.Hp >= 100))
                        {
                            Console.Write("체력 ");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(player.Hp.ToString() + " -> " + ( (player.Hp +50)>100? (player.Hp=100) : (player.Hp + 50)).ToString());
                            //HP 적용
                            player.Hp = (player.Hp + 50) > 100 ? (player.Hp = 100) : (player.Hp + 50);
                            InnMenu("체력을 회복했습니다.");
                        }
                        else InnMenu("체력이 최대치입니다.");                                                
                    }
                    // 3 : 돈이 모자라는 경우
                    else
                    {
                        InnMenu("Gold가 부족합니다.");
                    }
                    break;
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            GameManager gameManager = new GameManager();
            gameManager.StartGame();
        }
    }    
}
