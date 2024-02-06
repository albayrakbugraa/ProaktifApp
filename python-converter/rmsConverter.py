from datetime import datetime
import sys
import numpy as np
from comtrade import Comtrade

cfgfile = sys.argv[1]
datfile = sys.argv[2]
csvoutfile = sys.argv[3]

rec = Comtrade()
rec.load(cfgfile, datfile)

csvOutput = ">DATE(0/DATE), TIME(1/TIME),"

for i in range(len(rec.analog_channel_ids)):
    if i == len(rec.analog_channel_ids)-1: 
        csvOutput += rec.analog_channel_ids[i]
    else:
        csvOutput += rec.analog_channel_ids[i] + ","
        
window_size = 32

for i, r in enumerate(range(0, rec.total_samples)):
    csvOutput += "\n"
    dateStamp = rec.start_timestamp.timestamp() + rec.time[i]
    date = datetime.fromtimestamp(dateStamp)
    csvOutput += date.date().isoformat() + "," + date.time().isoformat() +","
    for j, x in enumerate(rec.analog_channel_ids):
        if i < window_size - 1: # ilk örneklem zamanları için önceki verileri alarak RMS hesapla
            data_window = rec.analog[j][0:i+1]
        else: # diğer örneklem zamanları için önceki 32 veriyi alarak RMS hesapla
            data_window = rec.analog[j][i-window_size+1:i+1]
        rms = np.sqrt(np.mean(np.square(data_window)))
        if j == len(rec.analog_channel_ids)-1: 
            csvOutput += "{:.3f}".format(rms)
        else:
            csvOutput += "{:.3f},".format(rms)

f = open(csvoutfile, "w")
f.write(csvOutput)
f.close()