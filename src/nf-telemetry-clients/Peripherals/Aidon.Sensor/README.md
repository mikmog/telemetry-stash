# Aidon.Sensor

https://aidon.com/wp-content/uploads/2023/06/AIDONFD_RJ12_HAN_Interface_SV.pdf

## Obis Codes

```
    OBIS            Beskrivning                                 Kommentar	         
    0-0.1.0.0	    Datum och tid                               Formatet YYMMDDhhmmss
    1-0:1.8.0	    Mätarställning Aktiv Energi Uttag		                         
    1-0:2.8.0	    Mätarställning Aktiv Energi Inmatning		                     
    1-0:3.8.0	    Mätarställning Reaktiv Energi Uttag		                         
    1-0:4.8.0	    Mätarställning Reaktiv Energi Inmatning		                     

    1-0:1.7.0	    Aktiv Effekt Uttag	                        Momentan trefaseffekt
    1-0:2.7.0	    Aktiv Effekt Inmatning	                    Momentan trefaseffekt

    1-0:3.7.0	    Reaktiv Effekt Uttag	                    Momentan trefaseffekt
    1-0:4.7.0	    Reaktiv Effekt Inmatning	                Momentan trefaseffekt

    1-0:21.7.0	    L1 Aktiv Effekt Uttag	                    Momentan effekt	     
    1-0:22.7.0	    L1 Aktiv Effekt Inmatning	                Momentan effekt	     
    1-0:41.7.0	    L2 Aktiv Effekt Uttag	                    Momentan effekt	     
    1-0:42.7.0	    L2 Aktiv Effekt Inmatning	                Momentan effekt	     
    1-0:61.7.0	    L3 Aktiv Effekt Uttag	                    Momentan effekt	     
    1-0:62.7.0	    L3 Aktiv Effekt Inmatning	                Momentan effekt	     

    1-0:23.7.0	    L1 Reaktiv Effekt Uttag	                    Momentan effekt	     
    1-0:24.7.0	    L1 Reaktiv Effekt Inmatning	                Momentan effekt	     
    1-0:43.7.0	    L2 Reaktiv Effekt Uttag	                    Momentan effekt	     
    1-0:44.7.0	    L2 Reaktiv Effekt Inmatning	                Momentan effekt	     
    1-0:63.7.0	    L3 Reaktiv Effekt Uttag	                    Momentan effekt	     
    1-0:64.7.0	    L3 Reaktiv Effekt Inmatning	                Momentan effekt	     

    1-0:32.7.0	    L1 Fasspänning	                            Momentant RMS-värde	 
    1-0:52.7.0	    L2 Fasspänning	                            Momentant RMS-värde	 
    1-0:72.7.0	    L3 Fasspänning	                            Momentant RMS-värde	 
    1-0:31.7.0	    L1 Fasström	                                Momentant RMS-värde	 
    1-0:51.7.0	    L2 Fasström	                                Momentant RMS-värde	 
    1-0:71.7.0	    L3 Fasström	                                Momentant RMS-värde	 
```

## Sample message

```
  /ADN9 6534

  0-0:1.0.0(240217174050W)
  1-0:1.8.0(00007805.332*kWh)
  1-0:2.8.0(00000000.000*kWh)
  1-0:3.8.0(00000003.239*kVArh)
  1-0:4.8.0(00001945.988*kVArh)
  1-0:1.7.0(0000.870*kW)
  1-0:2.7.0(0000.000*kW)
  1-0:3.7.0(0000.000*kVAr)
  1-0:4.7.0(0000.551*kVAr)
  1-0:21.7.0(0000.269*kW)
  1-0:22.7.0(0000.000*kW)
  1-0:41.7.0(0000.526*kW)
  1-0:42.7.0(0000.000*kW)
  1-0:61.7.0(0000.060*kW)
  1-0:62.7.0(0000.000*kW)
  1-0:23.7.0(0000.000*kVAr)
  1-0:24.7.0(0000.277*kVAr)
  1-0:43.7.0(0000.000*kVAr)
  1-0:44.7.0(0000.184*kVAr)
  1-0:63.7.0(0000.000*kVAr)
  1-0:64.7.0(0000.081*kVAr)
  1-0:32.7.0(234.7*V)
  1-0:52.7.0(234.6*V)
  1-0:72.7.0(234.5*V)
  1-0:31.7.0(001.6*A)
  1-0:51.7.0(002.3*A)
  1-0:71.7.0(000.4*A)
  !95CB
```