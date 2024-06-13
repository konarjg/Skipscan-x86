# Skipscan-x86

Moduł pomocniczy należy umieścić w tym samym folderze wraz z plikiem test.s oraz cmd.txt oraz uruchamiać w systemie Ubuntu poprzez kompilator g++.
Moduł główny uruchamiany jest w systemie Windows.

Opis modułu głównego:
Program wymaga wybrania jednej z 5 opcji:
1. Test DFS -> generowanie minimalnego zbioru testowego przy pomocy algorytmu Skipscan
2. Test leksykograficzny -> generowanie zbioru testowego przy pomocy bajtów w kolejności leksykograficznej (małe pokrycie)
3. Test losowy -> generowanie zbioru testowego w sposób losowy (mało wydajne)
4. Generowanie zredukowanego programu testowego -> generuje zredukowany o 35.54% program testowy ze zbioru stworzonego przez opcję 1
5. Wyjście -> zakańcza program
