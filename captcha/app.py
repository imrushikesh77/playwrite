import cv2
import numpy as np
import pytesseract
import easyocr

# Load image
image_path = 'textcaptcha5.jpeg'
image = cv2.imread(image_path)

# === Preprocessing for pytesseract ===
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
gray = cv2.medianBlur(gray, 3)

# Adaptive Thresholding
gray = cv2.adaptiveThreshold(
    gray, 255, 
    cv2.ADAPTIVE_THRESH_GAUSSIAN_C, 
    cv2.THRESH_BINARY_INV, 
    11, 2
)

# Morphology to clean up noise
kernel = np.ones((2, 2), np.uint8)
gray = cv2.morphologyEx(gray, cv2.MORPH_CLOSE, kernel)
gray = cv2.morphologyEx(gray, cv2.MORPH_OPEN, kernel)

# === Option 1: pytesseract ===
print("Using Tesseract OCR:")
custom_config = r'--oem 3 --psm 7 -c tessedit_char_whitelist=abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789'
text = pytesseract.image_to_string(gray, config=custom_config)
print("Detected CAPTCHA:", text.strip())

# === Option 2: easyocr ===
print("\nUsing EasyOCR:")
reader = easyocr.Reader(['en'])
results = reader.readtext(image_path)

for detection in results:
    bbox, text, confidence = detection
    print(f"Text: {text}, Confidence: {round(confidence, 2)}")
