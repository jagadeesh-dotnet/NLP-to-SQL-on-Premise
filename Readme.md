I wanted to share that what we've implemented so far is an initial step in the process and it is just an sample to understand how to generate SQL from NLP. It may not be perfect yet, but hope it serves as a foundation, and we can certainly improve in future.

Used the AdventureWorks DB for the example

https://github.com/Microsoft/sql-server-samples/releases/download/adventureworks/AdventureWorks2022.bak

Download, restore  and execute sales view


step 1: Excute Python script which downloads the T5 model (T5-LM-Large-text2sql-spider) from hugging face and run Flask server

step 2 : update the connection string & Run console app project in Visual studio and enter your queries with respect to schema

Single Table

![image](https://github.com/user-attachments/assets/5f56c7fa-7290-46bc-96ee-de7e3994a769)

![image](https://github.com/user-attachments/assets/cdc4dc27-ec3f-470b-a4a0-65bee0227b18)


Multiple Tables

![image](https://github.com/user-attachments/assets/417bf3df-1864-4646-80dc-1d9200eeb190)

