import sys
from datetime import datetime
from comtrade import Comtrade
import numpy as np

cfgfile = sys.argv[1]
datfile = sys.argv[2]
csvoutfile = sys.argv[3]

rec = Comtrade()
rec.load(cfgfile, datfile)

channel_prefixes_V = ["U", "V"]
channel_prefixes_I = ["IL", "I"]
channel_suffixes = ["1", "2", "3"]

V0_indices = []
for prefix in channel_prefixes_V:
    for suffix in channel_suffixes:
        channel_id = prefix + suffix
        if any(channel_id in x for x in rec.analog_channel_ids):
            index = rec.analog_channel_ids.index(channel_id)
            V0_indices.append(index)
            if len(V0_indices) == 3:
                break
    if len(V0_indices) == 3:
        break
else:
    V0_indices.extend([-1] * (3 - len(V0_indices)))

I0_indices = []
for prefix in channel_prefixes_I:
    for suffix in channel_suffixes:
        channel_id = prefix + suffix
        if any(channel_id in x for x in rec.analog_channel_ids):
            index = rec.analog_channel_ids.index(channel_id)
            I0_indices.append(index)
            if len(I0_indices) == 3:
                break
    if len(I0_indices) == 3:
        break
else:
    I0_indices.extend([-1] * (3 - len(I0_indices)))




# Sütunları bulamadığımız durumları kontrol et
missing_V0 = any(index == -1 for index in V0_indices)
missing_I0 = any(index == -1 for index in I0_indices)

if missing_V0 :
    I0_value = np.mean(np.array(rec.analog)[I0_indices], axis=0)
    csvOutput = ">DATE(0/DATE), TIME(1/TIME)," + ",".join(rec.analog_channel_ids) + ",I0"

    with open(csvoutfile, "w") as f:
        f.write(csvOutput)

        for i in range(rec.total_samples):
            dateStamp = rec.start_timestamp.timestamp() + rec.time[i]
            date = datetime.fromtimestamp(dateStamp)
            f.write("\n")
            f.write(date.date().isoformat() + "," + date.time().isoformat() + ",")

            for j, x in enumerate(rec.analog_channel_ids):
                f.write("{:.3f},".format(rec.analog[j][i]))

            f.write("{:.3f}".format(I0_value[i]))    

elif missing_I0 :
    V0_value = np.mean(np.array(rec.analog)[V0_indices], axis=0)
    csvOutput = ">DATE(0/DATE), TIME(1/TIME)," + ",".join(rec.analog_channel_ids) + ",V0"
    with open(csvoutfile, "w") as f:
        f.write(csvOutput)

        for i in range(rec.total_samples):
            dateStamp = rec.start_timestamp.timestamp() + rec.time[i]
            date = datetime.fromtimestamp(dateStamp)
            f.write("\n")
            f.write(date.date().isoformat() + "," + date.time().isoformat() + ",")

            for j, x in enumerate(rec.analog_channel_ids):
                f.write("{:.3f},".format(rec.analog[j][i]))

            f.write("{:.3f}".format(V0_value[i]))    

else:
    # V0 ve I0 değerlerini hesapla
    V0_value = np.mean(np.array(rec.analog)[V0_indices], axis=0)
    I0_value = np.mean(np.array(rec.analog)[I0_indices], axis=0)

    # CSV başlığı oluştur
    csvOutput = ">DATE(0/DATE), TIME(1/TIME)," + ",".join(rec.analog_channel_ids) + ",V0,I0"

    with open(csvoutfile, "w") as f:
        f.write(csvOutput)

        for i in range(rec.total_samples):
            dateStamp = rec.start_timestamp.timestamp() + rec.time[i]
            date = datetime.fromtimestamp(dateStamp)
            f.write("\n")
            f.write(date.date().isoformat() + "," + date.time().isoformat() + ",")

            for j, x in enumerate(rec.analog_channel_ids):
                f.write("{:.3f},".format(rec.analog[j][i]))

            f.write("{:.3f},{:.3f}".format(V0_value[i], I0_value[i]))