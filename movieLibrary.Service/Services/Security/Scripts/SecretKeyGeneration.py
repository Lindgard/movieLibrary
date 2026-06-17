import secrets
import base64
import os

# secrets is Python's cryptographically secure random number generator, which is ideal for generating secret keys
# base64 converts raw bytes into readable text, which is easier to store in environment variables and files

key = base64.b64encode(secrets.token_bytes(32)).decode("utf-8")

print(f"Generated key: {key}")

# os.path.expanduser("~") gives us the home directory path regardless of username or operating system
bashrc_path = os.path.expanduser("~/.bashrc")

with open(bashrc_path, "r") as f:
    existing_content = f.read()

if "JWT_SECRET_KEY" in existing_content:
    # input asks the user a yes/no question before overwriting
    overwrite = input("JWT_SECRET_KEY already exists in ~/.bashrc. Overwrite? (y/n): ")
    if overwrite.lower() != "y":
        print("Cancelled. No changes made.")
        exit() # exit stops the script immediately
    
    # Replace the old key line with the new one
    lines = existing_content.splitlines() # splitlines() splits the content into a list of lines
    new_lines = [line for line in lines if "JWT_SECRET_KEY" not in line] # filter out the old key line
    new_content = "\n".join(new_lines) + f'\nexport JWT_SECRET_KEY="{key}"\n'

    with open(bashrc_path, "w") as f: # "w" means write mode, which will overwrite the file
        f.write(new_content)
else:
    with open(bashrc_path, "a") as f: # "a" means append mode, which will add to the end of the file
        f.write(f'\nexport JWT_SECRET_KEY="{key}"\n')

os.environ["JWT_SECRET_KEY"] = key # set the environment variable for the current session

# Confirm it was set by reading it back
print(f"\nKey successfully set.")
print(f"Verification - JWT_SECRET_KEY is now: {os.environ.get('JWT_SECRET_KEY')}")
print(f"Key written to {bashrc_path} (persists across sessions)")